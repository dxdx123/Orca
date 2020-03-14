using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class EffectDirectionViewSystem : ReactiveSystem<GameEntity>
{
    public EffectDirectionViewSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.View, GameMatcher.Direction));
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasView && entity.hasEffect;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            var viewController = e.view.viewController;

            var direction = e.direction.direction;
            if (direction == CharacterDirection.Left || direction == CharacterDirection.Right)
            {
                viewController.displaySprite.scale = direction == CharacterDirection.Right
                    ? new Vector3(1, 1, 1)
                    : new Vector3(-1, 1, 1);
            }
            else
            {
                // nothing
            }
        }
    }
}
