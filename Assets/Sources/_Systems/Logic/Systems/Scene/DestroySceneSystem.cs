using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class DestroySceneSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;
    
    private IGroup<GameEntity> _viewGroup;
    private List<GameEntity> _cleanCache = new List<GameEntity>();
    
    public DestroySceneSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;

        _viewGroup = _gameContext.GetGroup(GameMatcher.View);
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Scene.Removed());
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        DestroyViewEntities();
    }

    private void DestroyViewEntities()
    {
        var entities = _viewGroup.GetEntities(_cleanCache);

        foreach (var e in entities)
        {
            e.isDestroy = true;
        }

        _gameContext.isCreateMap = false;
        _gameContext.isCreatePlayer = false;
    }
}
