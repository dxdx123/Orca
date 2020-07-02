using System;
using System.Collections;
using System.Collections.Generic;
using RSG;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetEditorLoader : IResourceLoader
{
    private static AssetEditorLoader _instance;

    public static AssetEditorLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AssetEditorLoader();
            }

            return _instance;
        }
    }
    
    private AssetEditorLoader()
    {}

    public IPromise<Object> BrandNewLoadAsset<T>(string path) where T : Object
    {
#if UNITY_EDITOR
        return new Promise<Object>((resolve, reject) =>
        {
            MainThreadDispatcher.StartUpdateMicroCoroutine(LoadAssetEditor<T>(path, resolve, reject));
        });
#else
        throw new NotImplementedException();
#endif
    }

#if UNITY_EDITOR
    private IEnumerator LoadAssetEditor<T>(string path, Action<Object> resolve, Action<Exception> reject) where T : Object
    {
        yield return null;

        T asset = AssetDatabase.LoadAssetAtPath<T>(path);

        if (asset)
        {
            resolve(asset);
        }
        else
        {
            reject(new Exception($"asset == null: {path}"));
        }
    }
#endif

    public void DestroyAsset(Object asset)
    {
        Assert.IsNotNull(asset);
        
        // nothing
    }
}
