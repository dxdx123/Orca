using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class DirectionSystem : ReactiveSystem<GameEntity>
{
    public DirectionSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Direction.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return  entity.hasView;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        for (int i = 0, length = entities.Count; i < length; ++i)
        {
            var e = entities[i];
            
            var sprite = e.view.viewController.displaySprite;
            var direction = e.direction.direction;

            sprite.FlipX = (direction == CharacterDirection.Left);
        }
    }
}
