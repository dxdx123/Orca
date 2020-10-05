using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendingActions
{
	public IAction[] CurrentActions;
	private IAction[] NextActions;
	private IAction[] NextNextActions;
	//incase other players advance to the next step and send their action before we advance a step
	private IAction[] NextNextNextActions;
	
	private int currentActionsCount;
	private int nextActionsCount;
	private int nextNextActionsCount;
	private int nextNextNextActionsCount;
	
	private PlayerLockStep _lsm;
	
	public PendingActions (PlayerLockStep lsm)
	{
		_lsm = lsm;
		
		CurrentActions = new IAction[lsm.NumberOfPlayers];
		NextActions = new IAction[lsm.NumberOfPlayers];
		NextNextActions = new IAction[lsm.NumberOfPlayers];
		NextNextNextActions = new IAction[lsm.NumberOfPlayers];
		
		currentActionsCount = 0;
		nextActionsCount = 0;
		nextNextActionsCount = 0;
		nextNextNextActionsCount = 0;
	}
	
	public void NextTurn()
	{
		//Finished processing this turns actions - clear it
		for(int i=0; i<CurrentActions.Length; i++)
		{
			CurrentActions[i] = null;
		}
		IAction[] swap = CurrentActions;
		
		//last turn's actions is now this turn's actions
		CurrentActions = NextActions;
		currentActionsCount = nextActionsCount;
		
		//last turn's next next actions is now this turn's next actions
		NextActions = NextNextActions;
		nextActionsCount = nextNextActionsCount;
		
		NextNextActions = NextNextNextActions;
		nextNextActionsCount = nextNextNextActionsCount;
		
		//set NextNextNextActions to the empty list
		NextNextNextActions = swap;
		nextNextNextActionsCount = 0;
	}
	
	public void AddAction(IAction action, int seatId, int currentLockStepTurn, int actionsLockStepTurn)
	{
		//add action for processing later
		if(actionsLockStepTurn == currentLockStepTurn + 1)
		{
			//if action is for next turn, add for processing 3 turns away
			if(NextNextNextActions[seatId] != null)
			{
				//TODO: Error Handling
				Debug.LogWarning("WARNING!!!! Recieved multiple actions for player " + seatId + " for turn "  + actionsLockStepTurn);
			}
			NextNextNextActions[seatId] = action;
			nextNextNextActionsCount++;
		}
		else if(actionsLockStepTurn == currentLockStepTurn)
		{
			//if recieved action during our current turn
			//add for processing 2 turns away
			if(NextNextActions[seatId] != null)
			{
				//TODO: Error Handling
				Debug.LogWarning("WARNING!!!! Recieved multiple actions for player " + seatId + " for turn "  + actionsLockStepTurn);
			}
			NextNextActions[seatId] = action;
			nextNextActionsCount++;
		}
		else if(actionsLockStepTurn == currentLockStepTurn - 1)
		{
			//if recieved action for last turn
			//add for processing 1 turn away
			if(NextActions[seatId] != null)
			{
				//TODO: Error Handling
				Debug.LogWarning("WARNING!!!! Recieved multiple actions for player " + seatId + " for turn "  + actionsLockStepTurn);
			}
			NextActions[seatId] = action;
			nextActionsCount++;
		}
		else
		{
			//TODO: Error Handling
			Debug.LogWarning("WARNING!!!! Unexpected lockstepID recieved : " + actionsLockStepTurn);
		}
	}
	
	public bool ReadyForNextTurn()
	{
		if(nextNextActionsCount == _lsm.NumberOfPlayers)
		{
			//if this is the 2nd turn, check if all the actions sent out on the 1st turn have been recieved
			if(_lsm.LockStepTurnID == PlayerLockStep.FirstLockStepTurnID + 1)
			{
				return true;
			}
			
			//Check if all Actions that will be processed next turn have been recieved
			if(nextActionsCount == _lsm.NumberOfPlayers)
			{
				return true;
			}
		}
		
		//if this is the 1st turn, no actions had the chance to be recieved yet
		if(_lsm.LockStepTurnID == PlayerLockStep.FirstLockStepTurnID)
		{
			return true;
		}
		
		//if none of the conditions have been met, return false
		return false;
	}
}
