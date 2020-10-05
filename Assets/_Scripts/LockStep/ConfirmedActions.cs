using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmedActions
{
    public List<int> playersConfirmedCurrentAction;
    public List<int> playersConfirmedPriorAction;
	
    private PlayerLockStep lsm;
    
    public ConfirmedActions (PlayerLockStep lsm)
    {
        this.lsm = lsm;
        playersConfirmedCurrentAction = new List<int>(lsm.NumberOfPlayers);
        playersConfirmedPriorAction = new List<int>(lsm.NumberOfPlayers);
    }
	
    public void NextTurn()
    {
        //clear prior actions
        playersConfirmedPriorAction.Clear ();
		
        List<int> swap = playersConfirmedPriorAction;
		
        //last turns actions is now this turns prior actions
        playersConfirmedPriorAction = playersConfirmedCurrentAction;
		
        //set this turns confirmation actions to the empty list
        playersConfirmedCurrentAction = swap;
    }
	
    public bool ReadyForNextTurn()
    {
        //check that the action that is going to be processed has been confirmed
        if(playersConfirmedPriorAction.Count == lsm.NumberOfPlayers)
        {
            return true;
        }
        //if 2nd turn, check that the 1st turns action has been confirmed
        if(lsm.LockStepTurnID == PlayerLockStep.FirstLockStepTurnID + 1)
        {
            return playersConfirmedCurrentAction.Count == lsm.NumberOfPlayers;
        }
        //no action has been sent out prior to the first turn
        if(lsm.LockStepTurnID == PlayerLockStep.FirstLockStepTurnID)
        {
            return true;
        }
        //if none of the conditions have been met, return false
        return false;
    }
}
