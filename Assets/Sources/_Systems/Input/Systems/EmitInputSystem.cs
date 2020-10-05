using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.InputSystem;

public class EmitInputSystem : IInitializeSystem, IExecuteSystem, ITearDownSystem
{
    private InputActions _inputActions;

    private InputContext _inputContext;
    
    private static readonly CharacterAction[] _characterActions = 
    {
        CharacterAction.LightAttack1,
        CharacterAction.LightAttack2, 
        CharacterAction.HeavyAttack1,
        CharacterAction.HeavyAttack2, 
        CharacterAction.LevelUp, 
        CharacterAction.Die, 
    };
    private InputAction[] _keyActions;
    
    public EmitInputSystem(Contexts contexts)
    {
        _inputActions = new InputActions();
        InitializeInputActions();

        _inputContext = contexts.input;
    }

    private void InitializeInputActions()
    {
        int length = _characterActions.Length;
        _keyActions = new InputAction[length];

        _keyActions[0] = _inputActions.Player.LightAttack1;
        _keyActions[1] = _inputActions.Player.LightAttack2;
        _keyActions[2] = _inputActions.Player.HeavyAttack1;
        _keyActions[3] = _inputActions.Player.HeavyAttack2;
        _keyActions[4] = _inputActions.Player.LevelUp;
        _keyActions[5] = _inputActions.Player.Die;
    }

    public void Initialize()
    {
        _inputActions.Enable();
    }

    public void Execute()
    {
        ProcessMove();
        ProcessAction();
    }

    private void ProcessAction()
    {
        int length = _characterActions.Length;

        for (int i = 0; i < length; ++i)
        {
            InputAction inputAction = _keyActions[i];
            if (inputAction.triggered)
            {
                CharacterAction action = _characterActions[i];
                // InputEntity inputEntity = _inputContext.CreateEntity();
                //
                // inputEntity.AddInputAction(action);
                
                var actionAction = new InputActionAction()
                {
                    characterAction = action,
                };
            
                LockStepController.Instance.AddAction(actionAction);

                // one Action per frame
                break;
            }
            else
            {
                // nothing
            }
        }
    }

    private void ProcessMove()
    {
        Vector2 move = _inputActions.Player.Move.ReadValue<Vector2>();

        if (Mathf.Approximately(move.x, 0.0f) && Mathf.Approximately(move.y, 0.0f))
        {
            // nothing
        }
        else
        { 
            // InputEntity inputEntity = _inputContext.CreateEntity();
            // inputEntity.AddInputMove(move.x, move.y);

            var moveAction = new InputMoveAction()
            {
                moveX = move.x,
                moveY = move.y,
            };
            
            LockStepController.Instance.AddAction(moveAction);
        }
    }

    public void TearDown()
    {
        _inputActions.Disable();
    }
}
