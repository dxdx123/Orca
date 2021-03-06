using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Entitas;
using UnityEngine;

public class EffectDestroySystem : IExecuteSystem
{
    private const float SIZE = 0.01f;

    private GameContext _gameContext;
    
    private IGroup<GameEntity> _effectGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public EffectDestroySystem(Contexts contexts)
    {
        _gameContext = contexts.game;
        
        _effectGroup = contexts.game.GetGroup(
            GameMatcher.AllOf(GameMatcher.Effect, GameMatcher.Position, GameMatcher.Target));
    }
    
    public void Execute()
    {
        var list = _effectGroup.GetEntities(_cleanBuffer);
        
        foreach (var entity in list)
        {
            GameEntity targetEntity = entity.target.target;
            var targetPos = targetEntity.position;
            float offsetY = targetEntity.view.viewController.displaySprite.CurrentSprite.GetUntrimmedBounds().extents.y;
            var destPos = new Vector2(targetPos.x, targetPos.y + offsetY);
            
            var srcPos = new Vector2(entity.position.x, entity.position.y);

            if((destPos - srcPos).sqrMagnitude < SIZE)
            {
                if (entity.hasAfterEffect)
                {
                    SpawnAfterEffect(entity.afterEffect.effect, destPos, entity.hasDirection ? entity.direction.direction : CharacterDirection.Left);

                    Attack(entity.spawner.spawner, targetEntity);
                }
                else
                {
                    Attack(entity.spawner.spawner, targetEntity);
                }
                
                entity.isDestroy = true;

            }
            else
            {
                // nothing
            }
        }
    }

    private void Attack(GameEntity attacker, GameEntity victim)
    {
        victim.ReplaceTarget(attacker);
        victim.behaviorTree.behaviorTree.SetVariable(
            "Master", (SharedGameObject)attacker.view.viewController.gameObject);
    }

    private void SpawnAfterEffect(string effectName, Vector2 destPos, CharacterDirection direction)
    {
        var e = _gameContext.CreateEntity();
        
        e.AddPosition(destPos.x, destPos.y);
        e.AddEffect(effectName);
        e.AddDirection(direction);
    }
}
