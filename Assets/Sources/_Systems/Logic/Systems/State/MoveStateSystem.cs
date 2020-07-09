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
        // Bugs: https://github.com/mzaks/EntitasCookBook/blob/master/chapters/1_ingredients/108_reactive_system.md
        return context.CreateCollector(GameMatcher.AttempMove.Added(), GameMatcher.State.Added());
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
