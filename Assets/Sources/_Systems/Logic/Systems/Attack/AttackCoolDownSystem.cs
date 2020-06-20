using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class AttackCoolDownSystem : IExecuteSystem
{
    private IGroup<GameEntity> _coolDownGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public AttackCoolDownSystem(Contexts contexts)
    {
        _coolDownGroup = contexts.game.GetGroup(GameMatcher.AttackCoolDown);
    }
    
    public void Execute()
    {
        List<GameEntity> entities = _coolDownGroup.GetEntities(_cleanBuffer);

        foreach (var e in entities)
        {
            e.attackCoolDown.coolDown -= Time.deltaTime;
        }
    }
}
