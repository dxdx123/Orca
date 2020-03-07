using Entitas.Unity;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory(ActionCategory.GameLogic)]
	public class CheckStateChange : FsmStateAction
	{

		public FsmGameObject _containerGo;

		private PlayMakerFSM _fsm;

		public override void OnEnter()
		{
			var entity = _containerGo.Value.GetEntityLink().entity as GameEntity;

			UnityEngine.Assertions.Assert.IsNotNull(entity);
			UnityEngine.Assertions.Assert.IsTrue(entity.hasFSM);
			UnityEngine.Assertions.Assert.IsTrue(entity.hasState);
			UnityEngine.Assertions.Assert.IsTrue(entity.hasView);

			_fsm = entity.fSM.fsm.fsm;
			
			entity.ReplaceAnimatorState(CharacterState.Idle);
		}

		public override void OnUpdate()
		{
			var entity = _containerGo.Value.GetEntityLink().entity as GameEntity;

			UnityEngine.Assertions.Assert.IsNotNull(entity);
			
			var newState = entity.state.state;
			if (newState.GetCacheString() != _fsm.FsmName)
			{
				_fsm.SendEvent(newState.GetCacheString());
			}
			else
			{
				// nothing
			}

		}
	}
}
