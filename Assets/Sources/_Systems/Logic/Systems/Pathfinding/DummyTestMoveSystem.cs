using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class DummyTestMoveSystem : IExecuteSystem
{
    private IGroup<GameEntity> _underControlGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();

    private bool _hasFindPath;
    
    public DummyTestMoveSystem(Contexts contexts)
    {
        _underControlGroup = contexts.game.GetGroup(GameMatcher.UnderControl);
    }
    
    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestMoveCharacter();
        }
    }

    private void TestMoveCharacter()
    {
        foreach (var e in _underControlGroup.GetEntities(_cleanBuffer))
        {
            float newX = e.position.x;
            float newY = e.position.y + 2;
            
            e.ReplaceFindPath(newX, newY, true);
            _hasFindPath = true;
        }
    }

    public void Cleanup()
    {
        if (_hasFindPath)
        {
            foreach (var e in _underControlGroup.GetEntities(_cleanBuffer))
            {
                e.RemoveFindPath();
            }

            _hasFindPath = false;
        }
        else
        {
            // nothing
        }
    }
}
