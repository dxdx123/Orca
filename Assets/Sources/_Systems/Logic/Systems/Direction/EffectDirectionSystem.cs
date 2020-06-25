using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class EffectDirectionSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;
    
    public EffectDirectionSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AttempDirection.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasAttempDirection && (entity.hasEffect || entity.hasAfterEffect);
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var entity in entities)
        {
            ChangeDirection(entity, entity.attempDirection.srcPosition, entity.attempDirection.destPosition);
            
            entity.RemoveAttempDirection();
        }
    }

    private void ChangeDirection(GameEntity entity, Vector2 srcPosition, Vector2 destPosition)
    {
        if (Mathf.Approximately(srcPosition.x, destPosition.x) &&
            Mathf.Approximately(srcPosition.y, destPosition.y))
        {
            // nothing
        }
        else
        {
            Vector2 diff = destPosition - srcPosition;

            if (diff.x > 0)
            {
                entity.ReplaceDirection(CharacterDirection.Right);
            }
            else if (diff.x < 0)
            {
                entity.ReplaceDirection(CharacterDirection.Left);
            }
            else
            {
                // nothing
            }
        }
    }
}
