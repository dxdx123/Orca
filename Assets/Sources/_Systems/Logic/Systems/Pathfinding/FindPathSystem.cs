using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class FindPathSystem : ReactiveSystem<GameEntity>
{
    public FindPathSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.FindPath.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasPosition;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            Vector3 oldPos = new Vector3(e.position.x, e.position.y, 0.0f);
            Vector3 newPos = new Vector3(e.findPath.x, e.findPath.y, 0.0f);

            if (e.hasPathfinding)
            {
                e.pathfinding.pathfinding.GotoDestination(e, oldPos, newPos, true);
            }
            else
            {
                EntityPathfinding pathfinding = PoolCacheManager.Instance.SpawnEntityPathfinding();
                pathfinding.GotoDestination(e, oldPos, newPos, true);
                
                e.AddPathfinding(pathfinding);
            }
        }
    }
}
