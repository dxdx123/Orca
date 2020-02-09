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

    private Rect _boundary;
    private bool _boundaryValid;
    
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

        Rect boundary = GetCameraBoundary();
        Vector3 newCameraPos = GetNewCameraPosition(boundary, position);
        // Vector3 newCameraPos = new Vector3(position.x, position.y, -10);

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

    private Vector3 GetNewCameraPosition(Rect boundary, Vector2 position)
    {
        float x = position.x;
        float y = position.y;

        float newX = Mathf.Clamp(x, boundary.xMin, boundary.xMax);
        float newY = Mathf.Clamp(y, boundary.yMin, boundary.yMax);
        Vector3 newCameraPos = new Vector3(newX, newY, -10);

        return newCameraPos;
    }

    private Rect GetCameraBoundary()
    {
        if (_boundaryValid)
        {
            return _boundary;
        }
        else
        {
            _boundary = GetCameraBoundaryInternal();
            _boundaryValid = true;

            return _boundary;
        }
    }

    private Rect GetCameraBoundaryInternal()
    {
        float halfHeight = _mainCamera.orthographicSize;
        float halfWidth = (float) Screen.width / Screen.height * halfHeight;

        float x = halfWidth;
        float y = halfHeight;

        var map = GetMapComponent();

        float mapPixelWidth = map.width;
        float mapPixelHeight = map.height;
        
        float width = mapPixelWidth / 100 - 2 * halfWidth;
        float height = mapPixelHeight / 100 - 2 * halfHeight;
        
        return new Rect(x, y, width, height);
    }

    private MapComponent GetMapComponent()
    {
        List<GameEntity> cleanCache = new List<GameEntity>(1);

        IGroup<GameEntity> mapGroup = _gameContext.GetGroup(GameMatcher.Map);
        List<GameEntity> entities = mapGroup.GetEntities(cleanCache);

        GameEntity entity = entities.SingleEntity();

        return entity.map;
    }
}
