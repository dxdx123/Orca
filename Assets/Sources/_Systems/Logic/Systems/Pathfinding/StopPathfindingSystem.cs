using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class StopPathfindingSystem : ReactiveSystem<GameEntity>
{
    public StopPathfindingSystem(Contexts contexts)
        : base(contexts.game)
    {
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AttempMove.Removed());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasPathfinding;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            e.pathfinding.pathfinding.Cleanup();

            SetupTargetEnemyInternal(e);
        }
    }

    private void SetupTargetEnemyInternal(GameEntity entity)
    {
        var underControlEntity = entity;
        GameEntity enemy = entity.target.target;

        if (enemy != null)
        {
            underControlEntity.ReplaceTarget(enemy);

            // direction
            float srcX = underControlEntity.position.x;
            float destX = enemy.position.x;

            underControlEntity.ReplaceDirection(destX >= srcX ? CharacterDirection.Right : CharacterDirection.Left);
        }
    }
}
