using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class LoadSceneSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;
    
    public LoadSceneSystem(Contexts contexts) 
        : base(contexts.game)
    {
        _gameContext = contexts.game;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Scene.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        _gameContext.isCreateMap = true;
        _gameContext.isCreatePlayer = true;
    }
}
