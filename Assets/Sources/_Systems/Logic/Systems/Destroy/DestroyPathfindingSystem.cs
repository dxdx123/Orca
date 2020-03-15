using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class DestroyPathfindingSystem : ReactiveSystem<GameEntity>
{
    public DestroyPathfindingSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Destroy.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasPathfinding;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            var pathfinding = e.pathfinding.pathfinding;
            
            pathfinding.Cleanup();
        }
    }
}
