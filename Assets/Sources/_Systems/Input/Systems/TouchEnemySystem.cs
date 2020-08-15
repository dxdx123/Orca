using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class TouchEnemySystem : ReactiveSystem<GameEntity>
{
    private IGroup<GameEntity> _underControlGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public TouchEnemySystem(Contexts contexts)
        : base(contexts.game)
    {
        _underControlGroup = contexts.game.GetGroup(GameMatcher.UnderControl);
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Touch.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasAI && entity.aI.type == AIType.Enemy;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        var input = entities.SingleEntity();

        CharacterAction action = CharacterAction.LightAttack1;
        CharacterState state = ParseActionState(action);
        
        foreach (var e in _underControlGroup.GetEntities(_cleanBuffer))
        {
            e.ReplaceState(state);
        }

        GameEntity enemyEntity = entities.SingleEntity();

        SetupTargetEnemy(action, enemyEntity);

        enemyEntity.isTouch = false;
    }
    
    private void SetupTargetEnemy(CharacterAction action, GameEntity enemyEntity)
    {
        switch (action)
        {
            case CharacterAction.LightAttack1:
            case CharacterAction.LightAttack2:
            case CharacterAction.HeavyAttack1:
            case CharacterAction.HeavyAttack2:
                SetupTargetEnemyInternal(enemyEntity);
                break;
            
            default:
                break;
        }
    }
    
    private void SetupTargetEnemyInternal(GameEntity enemyEntity)
    {
        var entities = _underControlGroup.GetEntities(_cleanBuffer);
        if (entities.Count == 1)
        {
            var underControlEntity = entities.SingleEntity();

            underControlEntity.ReplaceTarget(enemyEntity);
            
            // direction

            float srcX = underControlEntity.position.x;
            float destX = enemyEntity.position.x;
            
            underControlEntity.ReplaceDirection(destX >= srcX ? CharacterDirection.Right : CharacterDirection.Left);
        }
    }
    
    private CharacterState ParseActionState(CharacterAction action)
    {
        switch (action)
        {
            case CharacterAction.LightAttack1:
                return CharacterState.LightAttack1;
            
            case CharacterAction.LightAttack2:
                return CharacterState.LightAttack2;
            
            case CharacterAction.HeavyAttack1:
                return CharacterState.HeavyAttack1;
            
            case CharacterAction.HeavyAttack2:
                return CharacterState.HeavyAttack2;
            
            case CharacterAction.LevelUp:
                return CharacterState.LevelUp;
            
            case CharacterAction.Die:
                return CharacterState.Die;
            
            default:
                throw new Exception($"Invalid {action}");
        }
    }
}
