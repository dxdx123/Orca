using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class PositionSystem : ReactiveSystem<GameEntity>, ICleanupSystem
{
    private IGroup<GameEntity> _updateQuadTreeGroup;
    private List<GameEntity> _cleanCache = new List<GameEntity>();
    
    public PositionSystem(Contexts contexts)
        : base(contexts.game)
    {
        _updateQuadTreeGroup = contexts.game.GetGroup(GameMatcher.UpdateQuadTree);
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AnyOf(GameMatcher.Transform, GameMatcher.Position));
    }

    protected override bool Filter(GameEntity entity)
    {
        return entity.hasTransform && entity.hasPosition;
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach (var e in entities)
        {
            float x = e.position.x;
            float y = e.position.y;
            
            e.transform.transform.position = new Vector3(x, y, 0);
            
            // Update QuadTree
            if (e.isUnderControl)
            {
                e.AddUpdateQuadTree(x, y);
            }
            else
            {
                // nothing
            }
        }
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
