using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class EffectFollowSystem : IExecuteSystem
{
    private IGroup<GameEntity> _effectGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public EffectFollowSystem(Contexts contexts)
    {
        _effectGroup =
            contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Effect, GameMatcher.Position, GameMatcher.Target));
    }
    
    public void Execute()
    {
        var list = _effectGroup.GetEntities(_cleanBuffer);

        foreach (var entity in list)
        {
            var targetPos = entity.target.target.position;
            var destPos = new Vector2(targetPos.x, targetPos.y);
            
            var srcPos = new Vector2(entity.position.x, entity.position.y);

            var nextPos = Vector2.Lerp(srcPos, destPos, 0.1f);
            
            entity.ReplacePosition(nextPos.x, nextPos.y);
        }
    }
}
