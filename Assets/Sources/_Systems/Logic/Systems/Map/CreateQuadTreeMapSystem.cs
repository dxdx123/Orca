using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CreateQuadTreeMapSystem : ReactiveSystem<GameEntity>, ICleanupSystem
{
    private GameContext _gameContext;

    private IGroup<GameEntity> _updateQuadTreeGroup;
    private List<GameEntity> _cleanCache = new List<GameEntity>();
    
    public CreateQuadTreeMapSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;

        _updateQuadTreeGroup = _gameContext.GetGroup(GameMatcher.UpdateQuadTree);
    }
    
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.Map.Added());
    }

    protected override bool Filter(GameEntity entity)
    {
        return true;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        var entity = entities.SingleEntity();

        var map = entity.map;
        
        string mapName = map.mapName;
        float pixelWidth = map.width;
        float pixelHeight = map.height;
        
        QuadTreeMapManager.Instance.Initialize(_gameContext, mapName, pixelWidth, pixelHeight);
        
        // Update QuadTree After Initialize
        var position = GetUnderControllerCharacterPosition();
        entity.AddUpdateQuadTree(position.x, position.y);
    }

    private Vector2 GetUnderControllerCharacterPosition()
    {
        List<GameEntity> cleanCache = new List<GameEntity>(1);

        IGroup<GameEntity> mapGroup = _gameContext.GetGroup(
            GameMatcher.AllOf(GameMatcher.UnderControl, GameMatcher.Position));
        List<GameEntity> entities = mapGroup.GetEntities(cleanCache);

        GameEntity entity = entities.SingleEntity();
        
        return new Vector2(entity.position.x, entity.position.y);
    }

    public void Cleanup()
    {
        var list = _updateQuadTreeGroup.GetEntities(_cleanCache);

        foreach (var e in list)
        {
            e.RemoveUpdateQuadTree();
        }
    }
}
