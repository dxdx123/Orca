using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class DummyTestMoveSystem : IExecuteSystem
{
    private IGroup<GameEntity> _underControlGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();

    private GameContext _gameContext;
    
    public DummyTestMoveSystem(Contexts contexts)
    {
        _underControlGroup = contexts.game.GetGroup(GameMatcher.UnderControl);

        _gameContext = contexts.game;
    }
    
    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestCreateEffect();
        }

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

            float x = underControlEntity.position.x;
            float y = underControlEntity.position.y;

            var e = _gameContext.CreateEntity();

            e.AddPosition(x, y);
            e.AddSpeed(5.0f);
            e.AddEffect("hero_priest_bolt_fly");
            e.AddAfterEffect("hero_priest_bolt_destroy");
            e.AddTarget(underControlEntity.target.target);
        }
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
