using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResourceVisual : MonoBehaviour
{
    private List<Data> _cubeGoList = new List<Data>();
    private List<Data> _sphereGoList = new List<Data>();

    class Data
    {
        public GameObject go;
    }
    
    private void Start()
    {
        InitializeResourceManager();
    }

    private void InitializeResourceManager()
    {
        bool useBundle = true;
        ResourceManager.Instance.Initialize(useBundle);
        
        string path = $"{Application.streamingAssetsPath}/AssetBundles/AssetBundles";

        var manifestBundle = AssetBundle.LoadFromFile(path, 0, 0);
        if (!manifestBundle)
        {
            Debug.LogError("manifestBundle == null");
            return;
        }

        var manifestName = "AssetBundleManifest".ToLower();
        var manifest = manifestBundle.LoadAsset<AssetBundleManifest>(manifestName);

        string pathRoute = $"{Application.streamingAssetsPath}/AssetBundles/assets/res/core.assetbundle";
        var coreBundle = AssetBundle.LoadFromFile(pathRoute, 0, 0);
        if (!coreBundle)
        {
            Debug.LogError("routeAssetBundle == null");
            return;
        }
        
        string coreRoutePath = "Assets/Res/Core/AssetBundleRouteData.asset";
        var routeData = coreBundle.LoadAsset<AssetBundleRouteData>(coreRoutePath);
        if (!routeData)
        {
            Debug.LogError("routeData == null");
            return;
        }

        string variantPath = "Assets/Res/Core/AssetBundleVariantData.asset";
        var variantData = coreBundle.LoadAsset<AssetBundleVariantData>(variantPath);
        if (!variantData)
        {
            Debug.LogError("variantData == null");
            return;
        }
        
        var route = new AssetBundleRoute(routeData, variantData);
        ResourceManagerAssetBundle.Instance.Initialize(manifest, route); 
    }

    public void OnButton_LoadCube()
    {
        string path = "Assets/_Experiment/TestResourceVisual/Prefabs/Cube.prefab";
        
        var data = new Data()
        {
            go = null,
        };
        _cubeGoList.Add(data);

        int beginFrame = Time.frameCount;
        ResourceManager.Instance.GetAsset<GameObject>(path, this)
            .Then(asset =>
            {
                Debug.Log($"LoadAsync begin: {beginFrame} end: {Time.frameCount}");
                var cubeGo = Instantiate(asset);

                data.go = cubeGo;
            })
            .Catch(ex => Debug.LogException(ex));
    }
    
    public void OnButton_UnLoadCube()
    {
        if (_cubeGoList.Count > 0)
        {
            var cubeGo = _cubeGoList[0];
            _cubeGoList.RemoveAt(0);
            
            if(cubeGo.go != null)
                Destroy(cubeGo.go);
        
            string path = "Assets/_Experiment/TestResourceVisual/Prefabs/Cube.prefab";
            ResourceManager.Instance.DestroyAsset(path, this);
        }
    }
    
    public void OnButton_LoadSphere()
    {
        string path = "Assets/_Experiment/TestResourceVisual/Prefabs/Sphere.prefab";

        var data = new Data()
        {
            go = null,
        };
        _sphereGoList.Add(data);
        
        int beginFrame = Time.frameCount;
        ResourceManager.Instance.GetAssetSync<GameObject>(path, this)
            .Then(asset =>
            {
                Debug.Log($"LoadSync begin: {beginFrame} end: {Time.frameCount}");
                
                var sphereGo = Instantiate(asset);
                data.go = sphereGo;
            })
            .Catch(ex => Debug.LogException(ex));
    }
    
    public void OnButton_UnLoadSphere()
    {
        if (_sphereGoList.Count > 0)
        {
            var sphere = _sphereGoList[0];
            _sphereGoList.RemoveAt(0);
            
            if(sphere.go != null)
                Destroy(sphere.go);
        
            string path = "Assets/_Experiment/TestResourceVisual/Prefabs/Sphere.prefab";
            ResourceManager.Instance.DestroyAsset(path, this);
        }
    }
}
