using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CreateMapSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;

    public CreateMapSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Config.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        CreateMap();
    }

    private void CreateMap()
    {
        var mapConfigData = _gameContext.config.mapConfig;
        
        string mapName = "town";
        var mapConfig = mapConfigData.GetMapConfig(mapName);

        float width = mapConfig.width;
        float height = mapConfig.height;

        var mapEntity = _gameContext.CreateEntity();
        mapEntity.AddMap(mapName, width, height);
    }
}
