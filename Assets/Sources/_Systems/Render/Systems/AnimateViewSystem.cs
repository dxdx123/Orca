using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
public class AnimateViewSystem : ReactiveSystem<GameEntity>
{
    public AnimateViewSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AnimatorState.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasAnimatorState && entity.hasView;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            PlayAnimator(e);

            e.RemoveAnimatorState();
        }
    }

    private void PlayAnimator(GameEntity e)
    {
        var animator = e.view.viewController.displaySpriteAnimator;
        var stateString = e.animatorState.state.GetCacheString();

        animator.Play(stateString);
    }
}
