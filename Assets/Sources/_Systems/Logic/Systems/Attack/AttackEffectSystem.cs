using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Entitas;
using UnityEngine;

public class AttackEffectSystem : ReactiveSystem<GameEntity>
{
    public const string EFFECT_NAME = "hero_priest_bolt_fly";
    public const string AFTER_EFFECT_NAME = "hero_priest_bolt_destroy";

    public const float SPEED = 5.0f;
    
    private GameContext _gameContext;
    
    public AttackEffectSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.ViewEventTrigger.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasTarget;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            Vector2 offset = JsonUtility.FromJson<Vector2>(e.viewEventTrigger.eventInfo);
            
            SpawnAttackEffect(e, offset);
            
            e.RemoveViewEventTrigger();
        }
    }

    private void SpawnAttackEffect(GameEntity underControlEntity, Vector2 offset)
    {
        GameEntity enemy = underControlEntity.target.target;
        
        float x = underControlEntity.position.x;
        float y = underControlEntity.position.y;

        var e = _gameContext.CreateEntity();

        Vector2 newOffset = underControlEntity.direction.direction == CharacterDirection.Right
            ? new Vector2(offset.x, offset.y)
            : new Vector2(-offset.x, offset.y);
        
        e.AddPosition(x + newOffset.x, y + newOffset.y);
        e.AddSpeed(SPEED);
        e.AddEffect(EFFECT_NAME);
        e.AddAfterEffect(AFTER_EFFECT_NAME);
                
        e.AddTarget(enemy);
        e.AddSpawner(underControlEntity);
    }
}
