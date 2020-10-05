using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CharacterVelocitySystem : ReactiveSystem<GameEntity>
{
    public CharacterVelocitySystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Velocity.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasPosition;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            if (e.hasState)
            {
                ApplyVelocityCharacter(e);
            }
            else
            {
                ApplyVelocityEffect(e);
            }
        }
    }

    private void ApplyVelocityEffect(GameEntity e)
    {
        Vector2 velocity = new Vector2(e.velocity.x, e.velocity.y);

        bool flying = e.velocity.flying;
        ChangePosition(e, velocity, flying);
    }

    private void ApplyVelocityCharacter(GameEntity e)
    {
        if (e.state.state == CharacterState.Run)
        {
            Vector2 velocity = new Vector2(e.velocity.x, e.velocity.y);

            bool flying = e.velocity.flying;
            ChangePosition(e, velocity, flying);
        }
        else
        {
            // nothing
        }
    }

    private void ChangePosition(GameEntity e, Vector2 velocity, bool flying)
    {
        Vector2 distance = velocity * TimeManager.Instance.DeltaTime;

        Vector2 srcPosition = new Vector2(e.position.x, e.position.y);
        Vector2 destPosition = srcPosition + distance;

        if (flying)
        {
            e.ReplacePosition(destPosition.x, destPosition.y);
        }
        else
        {
            var info = AstarPath.active.GetNearest(new Vector3(destPosition.x, destPosition.y, 0.0f));
            e.ReplacePosition(info.position.x, info.position.y);
        }
        
        // change direction
        e.ReplaceAttempDirection(srcPosition, destPosition);
    }
}
