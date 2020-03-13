using System.Collections;
using System.Collections.Generic;
using DesperateDevs.Utils;
using Entitas;
using UnityEngine;

public class EffectVelocitySystem : IExecuteSystem, ICleanupSystem
{
    
    private IGroup<GameEntity> _effectGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public EffectVelocitySystem(Contexts contexts)
    {
        _effectGroup = contexts.game.GetGroup(
            GameMatcher.AllOf(GameMatcher.Effect, GameMatcher.Speed, GameMatcher.Position, GameMatcher.Target));
        
    }

    public void Execute()
    {
        var list = _effectGroup.GetEntities(_cleanBuffer);

        foreach (var entity in list)
        {
            var targetPos = entity.target.target.position;
            var destPos = new Vector2(targetPos.x, targetPos.y);
            
            var srcPos = new Vector2(entity.position.x, entity.position.y);

            var diff = (destPos - srcPos).normalized;
            var velocity = diff * entity.speed.speed;
            
            entity.AddVelocity(velocity.x, velocity.y, true);
        }
    }

    public void Cleanup()
    {
        var list = _effectGroup.GetEntities(_cleanBuffer);

        foreach (var entity in list)
        {
            if (entity.hasVelocity)
            {
                entity.RemoveVelocity();
            }
            else
            {
                // nothing
            }
        }
    }
}
