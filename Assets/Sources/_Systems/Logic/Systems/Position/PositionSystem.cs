using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class PositionSystem : ReactiveSystem<GameEntity>
{
    public PositionSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AnyOf(GameMatcher.Transform, GameMatcher.Position));
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasTransform && entity.hasPosition;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            float x = e.position.x;
            float y = e.position.y;
            
            e.transform.transform.position = new Vector3(x, y, 0);
        }
    }
}
