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

			PlaySprite(animator, state)
				.Then(() =>
				{
					entity.ReplaceState(ChacaterState.Idle);
					animator.Play(ChacaterState.Idle.GetCacheString());
					
					Finish();
				})
				.Catch(ex => Debug.LogException(ex));
		}

		private IPromise PlaySprite(tk2dSpriteAnimator animator, ChacaterState state)
		{
			string stateName = state.GetCacheString();
			var clip = animator.GetClipByName(stateName);
			if (clip == null)
				return Promise.Resolved();
			
			float duration = clip.frames.Length / clip.fps;
			
			return new Promise((resolve, reject) =>
			{
				 MainThreadDispatcher.StartUpdateMicroCoroutine(PlaySpriteInternal(resolve, reject, animator, state, duration));
			});
		}

		private IEnumerator PlaySpriteInternal(Action resolve, Action<Exception> reject, tk2dSpriteAnimator animator, ChacaterState state, float duration)
		{
			animator.Play(state.GetCacheString());
			
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
