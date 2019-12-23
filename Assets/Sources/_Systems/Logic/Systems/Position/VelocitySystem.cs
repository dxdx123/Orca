using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class VelocitySystem : ReactiveSystem<GameEntity>
{
    public VelocitySystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Velocity.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasPosition && entity.hasState;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            if (e.state.state == CharacterState.Run)
            {
                Vector2 velocity = new Vector2(e.velocity.x, e.velocity.y);
                Vector2 distance = velocity * Time.deltaTime;

                Vector2 srcPosition = new Vector2(e.position.x, e.position.y);
                Vector2 destPosition = srcPosition + distance;

                e.ReplacePosition(destPosition.x, destPosition.y);
            }
            else
            {
                // nothing
            }
        }
    }
}
