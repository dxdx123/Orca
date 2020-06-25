using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Entitas;
using UnityEngine;

public class AttackEffectSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;
    
    // private IGroup<GameEntity> _aiGroup;
    // private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public AttackEffectSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;
        
        // _aiGroup = contexts.game.GetGroup(GameMatcher.AI);
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
        e.AddSpeed(5.0f);
        e.AddEffect("hero_priest_bolt_fly");
        e.AddAfterEffect("hero_priest_bolt_destroy");
                
        e.AddTarget(enemy);
        
        // enemy.ReplaceTarget(underControlEntity);
        // enemy.behaviorTree.behaviorTree.SetVariable(
        //     "Master", (SharedGameObject)underControlEntity.view.viewController.gameObject);
    }
}
