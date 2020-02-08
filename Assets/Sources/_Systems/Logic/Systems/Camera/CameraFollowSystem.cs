using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CameraFollowSystem : IExecuteSystem
{
    public const float LERP_RATIO = 0.025f;
    
    private GameContext _gameContext;
    
    private IGroup<GameEntity> _cameraTargetGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();

    private Camera _mainCamera;
    private Transform _mainCameraTransform;
    
    public CameraFollowSystem(Contexts contexts)
    {
        _gameContext = contexts.game;

        _cameraTargetGroup = _gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.CameraTarget, GameMatcher.Position));

        _mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _mainCameraTransform = _mainCamera.transform;
    }
    
    public void Execute()
    {
        List<GameEntity> entities = _cameraTargetGroup.GetEntities(_cleanBuffer);
        if (entities.Count == 0)
            return;

        GameEntity entity = entities.SingleEntity();
        var positionComp = entity.position;
        
        Vector2 position = new Vector2(positionComp.x, positionComp.y);
        
        Vector3 oldCameraPos = _mainCameraTransform.position;
        // TODO:: check boundary
        Vector3 newCameraPos = new Vector3(position.x, position.y, -10);

        if (Mathf.Approximately(oldCameraPos.x, newCameraPos.x) && 
            Mathf.Approximately(oldCameraPos.y, newCameraPos.y))
        {
            // nothing
        }
        else
        {
            _mainCameraTransform.position = Vector3.Lerp(oldCameraPos, newCameraPos, LERP_RATIO);
        }
    }
}
