
using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Entitas;
using FMODUnity;
using UnityEngine;

public class TranslateInputActionSystem : ReactiveSystem<InputEntity>
{
    private GameContext _gameContext;

    private IGroup<GameEntity> _underControlGroup;
    private IGroup<GameEntity> _aiGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public TranslateInputActionSystem(Contexts contexts)
        : base(contexts.input)
    {
        _gameContext = contexts.game;

        _underControlGroup = _gameContext.GetGroup(GameMatcher.UnderControl);
        _aiGroup = _gameContext.GetGroup(GameMatcher.AI);
    }

    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.InputAction.Added());
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasInputAction;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        var input = entities.SingleEntity();

        CharacterAction action = input.inputAction.action;
        CharacterState state = ParseActionState(action);
        
        foreach (var e in _underControlGroup.GetEntities(_cleanBuffer))
        {
            e.ReplaceState(state);
        }

        SetupTargetEnemy(action);
    }

    private void SetupTargetEnemy(CharacterAction action)
    {
        switch (action)
        {
            case CharacterAction.LightAttack1:
            case CharacterAction.LightAttack2:
            case CharacterAction.HeavyAttack1:
            case CharacterAction.HeavyAttack2:
                SetupTargetEnemyInternal();
                break;
            
            default:
                break;
        }
    }
    
    private void SetupTargetEnemyInternal()
    {
        var entities = _underControlGroup.GetEntities(_cleanBuffer);
        if (entities.Count == 1)
        {
            var underControlEntity = entities.SingleEntity();
            GameEntity enemy = FindNearestEnemy(underControlEntity);

            if (enemy != null)
            {
                underControlEntity.ReplaceTarget(enemy);
                
                // direction
                float srcX = underControlEntity.position.x;
                float destX = enemy.position.x;
                
                underControlEntity.ReplaceDirection(destX >= srcX ? CharacterDirection.Right : CharacterDirection.Left);
                
                // moving
                underControlEntity.isAttempMove = false;
            }
            else
            {
                // nothing
            }
        }
    }
    
    private GameEntity FindNearestEnemy(GameEntity underControlEntity)
    {
        var entities = _aiGroup.GetEntities(_cleanBuffer);

        float minDistance = float.MaxValue;
        GameEntity enemy = null;

        Vector2 srcPos = new Vector2(underControlEntity.position.x, underControlEntity.position.y);
        
        foreach (var e in entities)
        {
            if (e.aI.type == AIType.Enemy)
            {
                Vector2 destPos = new Vector2(e.position.x, e.position.y);

                float distance = (destPos - srcPos).sqrMagnitude;

                if (distance < minDistance)
                {
                    enemy = e;
                    minDistance = distance;
                }
                else
                {
                    // nothing
                }
            }
            else
            {
                // nothing
            }
        }

        return enemy;
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