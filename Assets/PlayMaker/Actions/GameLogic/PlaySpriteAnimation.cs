using System;
using System.Collections;
using Entitas.Unity;
using RSG;
using UniRx;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory(ActionCategory.GameLogic)]
	public class PlaySpriteAnimation : FsmStateAction
	{
		public FsmGameObject containerGo;
		
		public override void OnEnter()
		{
			var gameObject = containerGo.Value;

			GameEntity entity = gameObject.GetEntityLink().entity as GameEntity;
			
			UnityEngine.Assertions.Assert.IsNotNull(entity);
			UnityEngine.Assertions.Assert.IsTrue(entity.hasState);
			UnityEngine.Assertions.Assert.IsTrue(entity.hasView);

			var animator = entity.view.viewController.displaySpriteAnimator;
			var state = entity.state.state;

			if (state == CharacterState.Idle)
			{
				throw new Exception("Can't Play Idle State Animation Here");
			}
			else if (state == CharacterState.Run)
			{
				// entity.ReplaceAnimatorState(state);
				MainThreadDispatcher.StartUpdateMicroCoroutine(WaitAndPlayRun(entity));
			}
			else
			{
				PlaySprite(entity, animator, state)
					.Then(() =>
					{
						Finish();

						if (entity.isEnabled)
						{
							entity.ReplaceState(CharacterState.Idle);
						}
						else
						{
							// nothing
						}
					})
					.Catch(ex => Debug.LogException(ex));
			}
		}

		private IEnumerator WaitAndPlayRun(GameEntity entity)
		{
			yield return null;
			entity.ReplaceAnimatorState(CharacterState.Run);
		}

		private IPromise PlaySprite(GameEntity entity, tk2dSpriteAnimator animator, CharacterState state)
		{
			string stateName = state.GetCacheString();
			var clip = animator.GetClipByName(stateName);
			
			float duration = clip.frames.Length / clip.fps;
			
			return new Promise((resolve, reject) =>
			{
				 MainThreadDispatcher.StartUpdateMicroCoroutine(PlaySpriteInternal(entity, resolve, reject, animator, state, duration));
			});
		}

		private IEnumerator PlaySpriteInternal(GameEntity entity, Action resolve, Action<Exception> reject, tk2dSpriteAnimator animator, CharacterState state, float duration)
		{
			entity.ReplaceAnimatorState(state);
			
			float time = 0.0f;

			while (time <= duration)
			{
				time += Time.deltaTime;
				yield return null;
			}

			resolve();
		}
	}

}
