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
            // TestMoveCharacter();
            TestCreateEffect();
        }
    }

    private void TestCreateEffect()
    {
        var entities = _underControlGroup.GetEntities(_cleanBuffer);
        var underControlEntity = entities.SingleEntity();

        float x = underControlEntity.position.x;
        float y = underControlEntity.position.y;

        var e = _gameContext.CreateEntity();
        
        e.AddPosition(x, y);
        e.AddEffect("hero_priest_shieldFx");
        
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
