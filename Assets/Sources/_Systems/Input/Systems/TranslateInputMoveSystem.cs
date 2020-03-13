using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class TranslateInputMoveSystem : ReactiveSystem<InputEntity>, ICleanupSystem
{
    public const float MULTI = 2.0f;
    
    private GameContext _gameContext;

    private IGroup<GameEntity> _underControlGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();

    private bool _dirty;
    
    public TranslateInputMoveSystem(Contexts contexts)
        : base(contexts.input)
    {
        _gameContext = contexts.game;

        _underControlGroup = _gameContext.GetGroup(GameMatcher.UnderControl);
    }
    
    protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
    {
        return context.CreateCollector(InputMatcher.InputMove.Added());
    }

    protected override bool Filter(InputEntity entity)
    {
        return entity.hasInputMove;
    }

    protected override void Execute(List<InputEntity> entities)
    {
        var input = entities.SingleEntity();

        float x = input.inputMove.x * MULTI;
        float y = input.inputMove.y * MULTI;
        
        foreach (var e in _underControlGroup.GetEntities(_cleanBuffer))
        {
            e.AddVelocity(x, y, false);
        }

        _dirty = true;
    }

    public void Cleanup()
    {
        if (_dirty)
        {
            foreach (var e in _underControlGroup.GetEntities(_cleanBuffer))
            {
                if(e.hasVelocity)
                    e.RemoveVelocity();
            }
            
            _dirty = false;
        }
        else
        {
            // nothing
        }
    }
}
