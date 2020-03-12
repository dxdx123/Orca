using System.Collections;
using System.Collections.Generic;
using Entitas;
using RSG;
using UnityEngine;

public class InitializeSystem : IInitializeSystem
{
    private const string PATH_SPRITE_CONFIG = "Assets/Res/Config/SpriteConfig.asset";
    private const string PATH_ANIMATOR_RUN_CONFIG = "Assets/Res/Config/AnimatorRunConfig.asset";
    private const string PATH_EFFECT_CONFIG = "Assets/Res/Config/EffectConfig.asset";
    
    private const string PATH_MAP_CONFIG = "Assets/Res/Config/MapConfig.asset";
    
    private GameContext _gameContext;

    private SpriteConfigData _spriteConfig;
    private AnimatorRunConfigData _animatorRunConfig;
    private EffectConfigData _effectConfig;
    
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

                return ResourceManager.Instance.GetAsset<AnimatorRunConfigData>(PATH_ANIMATOR_RUN_CONFIG, this);
            })
            .Then(configData =>
            {
                _animatorRunConfig = configData;
                _animatorRunConfig.Initialize();
                
                return ResourceManager.Instance.GetAsset<EffectConfigData>(PATH_EFFECT_CONFIG, this);
            })
            .Then(configData =>
            {
                _effectConfig = configData;
                _effectConfig.Initialize();
                
                return ResourceManager.Instance.GetAsset<MapConfigData>(PATH_MAP_CONFIG, this);
            })
            .Then(configData =>
            {
                _mapConfig = configData;
                _mapConfig.Initialize();
                
                _gameContext.SetConfig(_spriteConfig, _animatorRunConfig, _effectConfig, _mapConfig);

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
