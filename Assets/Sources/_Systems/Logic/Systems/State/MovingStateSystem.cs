
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class MovingStateSystem : ReactiveSystem<GameEntity>
{
    public MovingStateSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Moving.AddedOrRemoved());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasState && entity.hasFSM && entity.hasView;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        for (int i = 0, length = entities.Count; i < length; ++i)
        {
            var e = entities[i];

            var state = e.state.state;

            if (state == CharacterState.Run)
            {
                if (e.isMoving)
                {
                    // nothing
                }
                else
                {
                    e.ReplaceState(CharacterState.Idle);
                }
            }
            else if (state == CharacterState.Idle)
            {
                if (e.isMoving)
                {
                    e.ReplaceState(CharacterState.Run);
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