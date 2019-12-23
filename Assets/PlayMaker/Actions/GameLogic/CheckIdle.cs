using Entitas.Unity;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory(ActionCategory.GameLogic)]
	public class CheckIdle : FsmStateAction
	{
		public FsmGameObject _containerGo;
		
		private PlayMakerFSM _fsm;
		
		public override void OnEnter()
		{
			var entity = _containerGo.Value.GetEntityLink().entity as GameEntity;
			
			UnityEngine.Assertions.Assert.IsNotNull(entity);
			UnityEngine.Assertions.Assert.IsTrue(entity.hasFSM);

			_fsm = entity.fSM.fsm.fsm;
		}

		public override void OnUpdate()
		{
			var entity = _containerGo.Value.GetEntityLink().entity as GameEntity;
			
			UnityEngine.Assertions.Assert.IsNotNull(entity);

			if (entity.isMoving)
			{
				var newState = entity.state.state;
				if (newState != CharacterState.Run)
				{
					_fsm.SendEvent("FINISHED");
				}
				else
				{
					// nothing
				}
			}
			else
			{
				_fsm.SendEvent("FINISHED");
			}
		}

		private bool IsFirstPriorityState(CharacterState state)
		{
			if (state == CharacterState.LightAttack1
			    || state == CharacterState.LightAttack2
			    || state == CharacterState.HeavyAttack1
			    || state == CharacterState.HeavyAttack2
			    || state == CharacterState.Die
			    || state == CharacterState.LevelUp)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

	}

}
