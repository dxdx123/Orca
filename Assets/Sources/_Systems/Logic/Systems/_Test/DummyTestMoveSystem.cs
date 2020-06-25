using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Entitas;
using UnityEngine;

public class DummyTestMoveSystem : IExecuteSystem
{
    private IGroup<GameEntity> _underControlGroup;
    private IGroup<GameEntity> _enemyGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();

    private GameContext _gameContext;
    
    public DummyTestMoveSystem(Contexts contexts)
    {
        _underControlGroup = contexts.game.GetGroup(GameMatcher.UnderControl);
        _enemyGroup = contexts.game.GetGroup(GameMatcher.AI);

        _gameContext = contexts.game;
    }
    
    public void Execute()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     TestCreateEffect();
        // }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DestroyScene();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        _gameContext.SetScene("DefaultScene");
    }

    private void DestroyScene()
    {
        _gameContext.RemoveScene();
    }

    private void TestCreateEffect()
    {
        var entities = _underControlGroup.GetEntities(_cleanBuffer);
        if (entities.Count == 1)
        {
            var underControlEntity = entities.SingleEntity();
            GameEntity enemy = FindNearestEnemy(underControlEntity);

            if (enemy != null)
            {
                float x = underControlEntity.position.x;
                float y = underControlEntity.position.y;

                var e = _gameContext.CreateEntity();

                e.AddPosition(x, y);
                e.AddSpeed(5.0f);
                e.AddEffect("hero_priest_bolt_fly");
                e.AddAfterEffect("hero_priest_bolt_destroy");
                
                e.AddTarget(enemy);
                
                enemy.ReplaceTarget(underControlEntity);
                enemy.behaviorTree.behaviorTree.SetVariable(
                    "Master", (SharedGameObject)underControlEntity.view.viewController.gameObject);
            }
            else
            {
                // nothing
            }
        }
    }

    private GameEntity FindNearestEnemy(GameEntity underControlEntity)
    {
        var entities = _enemyGroup.GetEntities(_cleanBuffer);

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

    private void TestMoveCharacter()
    {
        foreach (var e in _underControlGroup.GetEntities(_cleanBuffer))
        {
            float newX = e.position.x;
            float newY = e.position.y + 2;
            
            e.ReplaceFindPath(newX, newY, true);
        }
    }
}
