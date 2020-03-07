using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class PathfindingPositionSystem : ReactiveSystem<GameEntity>
{
    public PathfindingPositionSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.PathfindingMove.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasPosition;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            var oldPos = new Vector3(e.position.x, e.position.y, 0.0f);
            var newPos = e.pathfindingMove.nextPosition;

            ChangeDirection(e, oldPos, newPos);
            
            e.ReplacePosition(newPos.x, newPos.y);
        }
    }

    private void ChangeDirection(GameEntity e, Vector3 oldPos, Vector3 newPos)
    {
        e.ReplaceAttempDirection(oldPos, newPos);
    }
}
