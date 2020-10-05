using System;
using System.Collections;
using System.Collections.Generic;
using Entitas;
using RSG;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

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
                return InitializeAssetPool();
            })
            .Then(() =>
            {
                return InitializeConfig();
            })
            .Then(() =>
            {
                // _gameContext.SetScene("DefaultScene");
                
                Debug.Log("!!! InitializeSystem finish");
            })
            .Catch(ex => Debug.LogException(ex));
    }

    private IPromise InitializePool()
    {
        return PoolCacheManager.Instance.WarmPathfinding(32);
    }

    private IPromise InitializeAssetPool()
    {
        return AssetPoolManager.Instance.Initialize(10, 10, 10, 64, 100);
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
#if UNITY_EDITOR
        bool useBundle = false;
        ResourceManager.Instance.Initialize(useBundle);
        
        return Promise.Resolved();
#else
        bool useBundle = true;
        ResourceManager.Instance.Initialize(useBundle);
        
        return new Promise((resolve, reject) =>
        {
            MainThreadDispatcher.StartUpdateMicroCoroutine(InitializeAssetBundle(resolve, reject));
        });        
#endif
    }

    private IEnumerator InitializeAssetBundle(Action resolve, Action<Exception> reject)
    {
        AssetBundleManifest assetBundleManifest = null;
        {
            string manifestPath = $"{Application.streamingAssetsPath}/AssetBundles/AssetBundles";
            var manifestABReq = AssetBundle.LoadFromFileAsync(manifestPath);
            while (!manifestABReq.isDone)
                yield return null;

            var manifestAssetBundle = manifestABReq.assetBundle;
            if (manifestAssetBundle != null)
            {
                string manifestName = "AssetBundleManifest";
                var assetReq = manifestAssetBundle.LoadAssetAsync<AssetBundleManifest>(manifestName);

                while (!assetReq.isDone)
                    yield return null;

                assetBundleManifest = assetReq.asset as AssetBundleManifest;
                Assert.IsNotNull(assetBundleManifest);
            }
            else
            {
                reject(new Exception("manifest assetbundle == null"));
                yield break;
            }
        }

        AssetBundleRouteData assetBundleRouteData = null;
        AssetBundleVariantData assetBundleVariantData = null;
        {
            string corePath = $"{Application.streamingAssetsPath}/AssetBundles/assets/res/core.assetbundle";
            
            var coreABReq = AssetBundle.LoadFromFileAsync(corePath);
            while (!coreABReq.isDone)
                yield return null;

            var coreAssetBundle = coreABReq.assetBundle;
            if (coreAssetBundle != null)
            {
                string routeName = "Assets/Res/Core/AssetBundleRouteData.asset";
                var routeReq = coreAssetBundle.LoadAssetAsync<AssetBundleRouteData>(routeName);

                while (!routeReq.isDone)
                    yield return null;

                assetBundleRouteData = routeReq.asset as AssetBundleRouteData;
                Assert.IsNotNull(assetBundleRouteData);
                
                string variantName = "Assets/Res/Core/AssetBundleVariantData.asset";
                var variantReq = coreAssetBundle.LoadAssetAsync<AssetBundleVariantData>(variantName);

                while (!variantReq.isDone)
                    yield return null;

                assetBundleVariantData = variantReq.asset as AssetBundleVariantData;
                Assert.IsNotNull(assetBundleVariantData);
                
                ResourceManagerAssetBundle.Instance.Initialize(assetBundleManifest, new AssetBundleRoute(assetBundleRouteData, assetBundleVariantData));

                resolve();
            }
            else
            {
                reject(new Exception("manifest assetbundle == null"));
            }
        }
    }
}
