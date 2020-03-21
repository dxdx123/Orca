using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Assertions;

public class TouchScreenSystem : IInitializeSystem, ITearDownSystem
{
    private GameContext _gameContext;

    private TapGesture _tapGesture;
    
    private Camera _camera;
    private Transform _cameraTransform;
    
    private IGroup<GameEntity> _underControlGroup;
    private readonly List<GameEntity> _cleanBuffer = new List<GameEntity>();
    
    public TouchScreenSystem(Contexts contexts)
    {
        _gameContext = contexts.game;
        
        _underControlGroup = contexts.game.GetGroup(GameMatcher.UnderControl);
    }
    
    public void Initialize()
    {
        var cameraGo = GameObject.FindGameObjectWithTag("MainCamera");
        Assert.IsNotNull(cameraGo);

        _camera = cameraGo.GetComponent<Camera>();
        _cameraTransform = cameraGo.transform;
        
        _tapGesture = cameraGo.GetComponent<TapGesture>();
        Assert.IsNotNull(_tapGesture);

        _tapGesture.Tapped += OnScreenTapped;
    }

    private void OnScreenTapped(object sender, EventArgs args)
    {
        TapGesture tapGesture = sender as TapGesture;
        Assert.IsNotNull(tapGesture);

        Vector2 screenPos = tapGesture.ScreenPosition;
        Vector2 worldPos = TranslateScreenPositionToWorldPosition(screenPos);
        
        foreach (var e in _underControlGroup.GetEntities(_cleanBuffer))
        {
            e.ReplaceFindPath(worldPos.x, worldPos.y, true);
        }
    }

    private Vector2 TranslateScreenPositionToWorldPosition(Vector2 screenPos)
    {
        var heightSize = _camera.orthographicSize * 2;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        Vector2 cameraWorldSize = new Vector2( heightSize * screenWidth / screenHeight,heightSize);
        
        var halfPixel = new Vector2(screenWidth / 2, screenHeight / 2);

        Vector2 diffPixel = screenPos - halfPixel;
        Vector2 diffLogic = new Vector2(diffPixel.x * cameraWorldSize.x / screenWidth, diffPixel.y * cameraWorldSize.y / screenHeight);

        Vector2 cameraPosition = _cameraTransform.position;
        
        return cameraPosition + diffLogic;
    }

    public void TearDown()
    {
        _tapGesture.Tapped -= OnScreenTapped;
    }
}
