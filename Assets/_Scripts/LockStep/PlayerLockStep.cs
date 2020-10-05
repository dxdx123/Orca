using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;
using SLNet;
using UnityEngine;

public class PlayerLockStep
{
    public const int FirstLockStepTurnID = 0;
    
    private float _accumilatedTime;

    public const int GameFramesPerLockstepTurn = 4;
    private int _gameFrame;

    private int _lockStepTurnID;
    public int LockStepTurnID
    {
        get
        {
            return _lockStepTurnID;
        }

        set
        {
            _lockStepTurnID = value;
        }
    }

    public int NumberOfPlayers = 1;

    private ConfirmedActions _confirmedActions;
    private PendingActions _pendingActions;
    
    private Queue<IAction> _actionsToSend;

    private LockStepController _lockStepController;
    private bool _logEnable;
    
    public PlayerLockStep(LockStepController lockStepController, bool logEnable)
    {
        _lockStepController = lockStepController;
        _logEnable = logEnable;
        
        PrepGameStart();
    }
    
    private void PrepGameStart()
    {
        LockStepTurnID = FirstLockStepTurnID;
        
        _pendingActions = new PendingActions(this);
        _confirmedActions = new ConfirmedActions(this);
        _actionsToSend = new Queue<IAction>();
    }

    public void OnUpdate(float deltaTime)
    {
        _accumilatedTime += deltaTime;

        while (_accumilatedTime >= TimeManager.FrameLength)
        {
            GameFrameTurn();
            _accumilatedTime -= TimeManager.FrameLength;
        }
    }

    private void GameFrameTurn()
    {
        // if(_logEnable)
        //     Debug.Log($"!!! gameFrameTurn {Time.frameCount}");
        
        if (_gameFrame == 0)
        {
            if (LockStepTurn())
            {
                if(_logEnable)
                    Debug.LogWarning($"!!! LockStepTurn True {Time.frameCount}");
                
                _gameFrame++;
            }
            else
            {
                // not ready
                if(_logEnable)
                    Debug.LogWarning($"!!! LockStepTurn False {Time.frameCount}");
            }
        }
        else
        {
            // Update Game
            // ...
            
            _lockStepController.OnGameTurn();

            _gameFrame++;
            _gameFrame %= GameFramesPerLockstepTurn;
        }
    }

    private bool LockStepTurn()
    {
        bool nextTurn = NextTurn();
        
        if(nextTurn)
        {
            SendPendingAction();
            //the first and second lockstep turn will not be ready to process yet
            if(LockStepTurnID >= FirstLockStepTurnID + 3)
            {
                ProcessActions();
            }
            else
            {
                // no action
            }

            return true;
        }
        else
        {
            return false;
        }
    }
    
    
    private bool NextTurn()
    {
        if (_confirmedActions.ReadyForNextTurn() && _pendingActions.ReadyForNextTurn())
        {
            LockStepTurnID++;

            _confirmedActions.NextTurn();
            _pendingActions.NextTurn();

            return true;
        }
        else
        {
            return false;
        }
    }
    
    private void ProcessActions()
    {
        if(_logEnable)
            Debug.LogError($"Process at: {LockStepTurnID} frame: {Time.frameCount}");
        
        foreach(IAction action in _pendingActions.CurrentActions)
        {
            action.ProcessAction(_logEnable);
        }
    }

    private void SendPendingAction()
    {
        int seatId = NetworkController.Instance.SeatId;
        
        IAction action = _actionsToSend.Count > 0 ? _actionsToSend.Dequeue() : new NoAction();
        
        //add action to our own list of actions to process
        _pendingActions.AddAction(action, seatId, LockStepTurnID, LockStepTurnID);
        
        //confirm our own action
        _confirmedActions.playersConfirmedCurrentAction.Add(seatId);
        
        //send action to all other players
        SendToOtherReceiveAction(LockStepTurnID, action);
    }

    private void SendToOtherReceiveAction(int lockStepTurnID, IAction action)
    {
        int actionId = GetActionId(action);
        byte[] bytes = GetBytes(action);

        LockstepPacketReq obj = new LockstepPacketReq()
        {
            LockstepTurnID = lockStepTurnID,

            ActionId = actionId,
            ActionData = bytes,
        };
        
        NetworkController.Instance.SendObjectRaw((byte)DefaultMessageIDTypes.ID_USER_SEND_ACTION_REQ, obj); 
    }

    private int GetActionId(IAction action)
    {
        if (action is NoAction)
        {
            return 0;
        }
        else if (action is InputMoveAction)
        {
            return 1;
        }
        else if (action is InputActionAction)
        {
            return 2;
        }
        else
        {
            throw new Exception($"Invalid {action}");
        }
    }

    private byte[] GetBytes(IAction action)
    {
        if (action is NoAction)
        {
            return MessagePackSerializer.Serialize(action as NoAction);
        }
        else if (action is InputMoveAction)
        {
            return MessagePackSerializer.Serialize(action as InputMoveAction);
        }
        else if (action is InputActionAction)
        {
            return MessagePackSerializer.Serialize(action as InputActionAction);
        }
        else
        {
            throw new Exception($"Invalid {action}");
        }
    }
    
    private IAction ParseAction(int actionId, byte[] actionData)
    {
        switch (actionId)
        {
            case 0:
                return MessagePackSerializer.Deserialize<NoAction>(actionData);
                
            case 1:
                return MessagePackSerializer.Deserialize<InputMoveAction>(actionData);
                
            case 2:
                return MessagePackSerializer.Deserialize<InputActionAction>(actionData);
                
            
            default:
                throw new Exception($"Invalid actionId: {actionId}");
        }
    }

    public void OnOtherUserSendActionRes(LockstepPacketRes res)
    {
        int actionId = res.ActionId;
        byte[] actionData = res.ActionData;

        IAction action = ParseAction(actionId, actionData);
        
        ReceiveAction(res.LockstepTurnID, res.PlayerID, res.SeatID, action);
    }

    public void ReceiveAction(int lockStepTurn, string senderPlayerID, int senderSeatId, IAction action)
    {
        Debug.Log($"Received Player {Time.frameCount} " + senderSeatId + "'s action for turn " + lockStepTurn + " on turn " + LockStepTurnID);
        
        if(action == null)
        {
            Debug.Log("Sending action failed");
            //TODO: Error handle invalid actions receive
        }
        else
        {
            _pendingActions.AddAction(action, senderSeatId, LockStepTurnID, lockStepTurn);
			     
            SendConfirmActionServer(lockStepTurn, senderPlayerID);
        }
    }

    private void SendConfirmActionServer(int lockStepTurn, string senderPlayerID)
    {
        int seatId = NetworkController.Instance.SeatId;
        
        LockstepConformReq req = new LockstepConformReq()
        {
            LockstepTurnID = lockStepTurn,
            PlayerID = senderPlayerID,
            
            SeatID = seatId,
        };
        
        NetworkController.Instance.SendObjectRaw((byte)DefaultMessageIDTypes.ID_USER_CONFIRM_ACTION_REQ, req);
    }

    public void OnServerConfirmAction(LockstepConformRes res)
    {
        ConfirmAction(res.LockstepTurnID, res.PlayerID, res.SeatID);
    }

    public void ConfirmAction(int lockStepTurn, string confirmingPlayerID, int confirmingSeatID)
    {
        //log.Debug ("Player " + confirmingPlayerID + " confirmed action for turn " + lockStepTurn + " on turn " + LockStepTurnID);
        if(lockStepTurn == LockStepTurnID)
        {
            //if current turn, add to the current Turn Confirmation
            _confirmedActions.playersConfirmedCurrentAction.Add (confirmingSeatID);
        }
        else if(lockStepTurn == LockStepTurnID - 1)
        {
            //if confirmation for prior turn, add to the prior turn confirmation
            _confirmedActions.playersConfirmedPriorAction.Add (confirmingSeatID);
        }
        else
        {
            //TODO: Error Handling
            Debug.LogWarning("WARNING!!!! Unexpected lockstepID Confirmed : " + lockStepTurn + " from player: " + confirmingPlayerID);
        }
    }

    public void AddAction(IAction action)
    {
        if (_actionsToSend.Count == 0)
        {
            _actionsToSend.Enqueue(action);
        }
        else
        {
            // nothing
        }
    }
}
