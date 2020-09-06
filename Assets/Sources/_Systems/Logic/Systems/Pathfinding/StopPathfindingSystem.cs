using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class StopPathfindingSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;

    private IGroup<GameEntity> _underControlGroup;
    private IGroup<GameEntity> _aiGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public StopPathfindingSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;

        _underControlGroup = _gameContext.GetGroup(GameMatcher.UnderControl);
        _aiGroup = _gameContext.GetGroup(GameMatcher.AI);
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
        GameEntity enemy = FindNearestEnemy(underControlEntity);

        if (enemy != null)
        {
            underControlEntity.ReplaceTarget(enemy);

            // direction
            float srcX = underControlEntity.position.x;
            float destX = enemy.position.x;

            underControlEntity.ReplaceDirection(destX >= srcX ? CharacterDirection.Right : CharacterDirection.Left);
        }
    }
    
    private GameEntity FindNearestEnemy(GameEntity underControlEntity)
    {
        var entities = _aiGroup.GetEntities(_cleanBuffer);

        float minDistance = float.MaxValue;
        GameEntity enemy = null;

        Vector2 srcPos = new Vector2(underControlEntity.position.x, underControlEntity.position.y);
        
        foreach (var e in entities)
        {
            if (e.aI.type == AIType.Enemy)
            {
                Vector2 destPos = new Vector2(e.position.x, e.position.y);

                float distance = (destPos - srcPos).sqrMagnitude;

                if (distance < minDistance)
                {
                    enemy = e;
                    minDistance = distance;
                }
                else
                {
                    // nothing
                }
            }
            else
            {
                // nothing
            }
        }

        return enemy;
    }
}
