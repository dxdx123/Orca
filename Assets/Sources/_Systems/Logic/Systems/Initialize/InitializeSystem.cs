using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class InitializeSystem : IInitializeSystem
{
    private const string PATH_SPRITE_CONFIG = "Assets/Res/Config/SpriteConfig.asset";
    
    private GameContext _gameContext;
    
    public InitializeSystem(Contexts contexts)
    {
        _gameContext = contexts.game;
    }
    
    public void Initialize()
    {
        InitializeResourceManager();
        InitializeConfig();
    }

    private void InitializeConfig()
    {
        ResourceManager.Instance.GetAsset<SpriteConfigData>(PATH_SPRITE_CONFIG, this)
            .Then(configData =>
            {
                configData.Initialize();
                _gameContext.SetConfig(configData);
            })
            .Catch(ex => Debug.LogException(ex));
    }

    private void InitializeResourceManager()
    {
    }
}
