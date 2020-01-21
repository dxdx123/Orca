using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class MoveStateSystem : ReactiveSystem<GameEntity>
{
    public MoveStateSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AnyOf(GameMatcher.AttempMove, GameMatcher.State));
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasState;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            if (e.isAttempMove)
            {
                if (e.state.state == CharacterState.Run)
                {
                    e.isMoving = true;
                }
                else
                {
                    e.isMoving = false;
                }
            }
            else
            {
                e.isMoving = false;
            }
        }
    }
}
