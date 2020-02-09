using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Assertions;

public class UpdateQuadTreeMapSystem : ReactiveSystem<GameEntity>
{
    private GameContext _gameContext;
    
    public UpdateQuadTreeMapSystem(Contexts contexts)
        : base(contexts.game)
    {
        _gameContext = contexts.game;
    }
    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.UpdateQuadTree);
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasUpdateQuadTree;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        var entity = entities.SingleEntity();

        Vector2 position = new Vector2(entity.updateQuadTree.x, entity.updateQuadTree.y);
        QuadTreeMapManager.Instance.Update(position);
    }
}
