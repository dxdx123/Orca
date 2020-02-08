using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using UnityEngine.Assertions;

public class UpdateQuadTreeMapSystem : IExecuteSystem
{
    private GameContext _gameContext;
    
    private IGroup<GameEntity> _cameraTargetGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public UpdateQuadTreeMapSystem(Contexts contexts)
    {
        _gameContext = contexts.game;

        _cameraTargetGroup = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.CameraTarget, GameMatcher.Position));
    }
    
    public void Execute()
    {
        List<GameEntity> entities = _cameraTargetGroup.GetEntities(_cleanBuffer);
        if (entities.Count == 0)
            return;

        GameEntity entity = entities.SingleEntity();

        var positionComp = entity.position;
        
        Vector2 position = new Vector2(positionComp.x, positionComp.y);
        
        QuadTreeMapManager.Instance.Update(position);
    }
}
