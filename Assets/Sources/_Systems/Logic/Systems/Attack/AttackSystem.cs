using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class AttackSystem : ReactiveSystem<GameEntity>
{
    public AttackSystem(Contexts contexts)
        : base(contexts.game)
    {
        
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Attack.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasTarget && entity.hasAttackCoolDown && entity.hasAttackInterval;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            if (e.attackCoolDown.coolDown > 0)
            {
                // still cool down
                e.isAttack = false;
            }
            else
            {
                e.ReplaceState(CharacterState.LightAttack1);
                e.isAttack = false;
                e.attackCoolDown.coolDown = e.attackInterval.interval;
            }
        }
    }
}
