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
        return context.CreateCollector(GameMatcher.AttempDirection.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasAttempDirection;
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
            
            if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
            {
                // move y
                if (diff.y > 0)
                {
                    entity.ReplaceDirection(CharacterDirection.Up);
                }
                else if (diff.y < 0)
                {
                    entity.ReplaceDirection(CharacterDirection.Down);
                }
                else
                {
                    // nothing
                }
            }
            else if(Mathf.Abs(diff.y) < Mathf.Abs(diff.x))
            {
                // move x
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
            else
            {
                // nothing
            }
        }
    }
}
