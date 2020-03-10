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
        return entity.hasTarget;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            e.ReplaceState(CharacterState.LightAttack1);

            e.isAttack = false;
        }
    }
}
