using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class DestroyMapSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;
    
    public DestroyMapSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.CreateMap.Removed());
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        _gameContext.RemoveMap();
    }
}
