﻿using System.Collections;
using System.Collections.Generic;
using Entitas;
using RSG;
using UnityEngine;

public class InitializeSystem : IInitializeSystem
{
    private const string PATH_SPRITE_CONFIG = "Assets/Res/Config/SpriteConfig.asset";
    private const string PATH_MAP_CONFIG = "Assets/Res/Config/MapConfig.asset";
    
    private GameContext _gameContext;

    private SpriteConfigData _spriteConfig;
    private MapConfigData _mapConfig;
    
    public InitializeSystem(Contexts contexts)
    {
        _gameContext = contexts.game;
    }
    
    public void Initialize()
    {
        InitializeResourceManager()
            .Then(() =>
            {
                return InitializePool(); 
            })
            .Then(() =>
            {
                return InitializeConfig();
            })
            .Catch(ex => Debug.LogException(ex));
    }

    private IPromise InitializePool()
    {
        return PoolCacheManager.Instance.WarmPathfinding(32);
    }

    private IPromise InitializeConfig()
    {
        var promise = new Promise();
        
        ResourceManager.Instance.GetAsset<SpriteConfigData>(PATH_SPRITE_CONFIG, this)
            .Then(configData =>
            {
                _spriteConfig = configData;
                _spriteConfig.Initialize();

                return ResourceManager.Instance.GetAsset<MapConfigData>(PATH_MAP_CONFIG, this);
            })
            .Then(configData =>
            {
                _mapConfig = configData;
                _mapConfig.Initialize();
                
                _gameContext.SetConfig(_spriteConfig, _mapConfig);

                promise.Resolve();
            })
            .Catch(ex =>
            {
                Debug.LogException(ex);
                promise.Reject(ex);
            });

        return promise;
    }

    private IPromise InitializeResourceManager()
    {
        return Promise.Resolved();
    }
}
