using System;
using System.Collections;
using System.Collections.Generic;
using RSG;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

public class AssetWebRequestTextureLoader : IResourceLoader
{
    private static AssetWebRequestTextureLoader _instance;

    public static AssetWebRequestTextureLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AssetWebRequestTextureLoader();
            }

            return _instance;
        }
    }
    
    private AssetWebRequestTextureLoader()
    {}


    public IPromise<Object> BrandNewLoadAsset<T>(string path) where T : Object
    {
        return new Promise<Object>((resolve, reject) =>
        {
            MainThreadDispatcher.StartUpdateMicroCoroutine(LoadTexture(path, resolve, reject));
        });
    }

    private IEnumerator LoadTexture(string url, Action<Object> resolve, Action<Exception> reject)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            var request = uwr.SendWebRequest();
            while (!request.isDone)
                yield return null;

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                reject(new Exception(uwr.error));
            }
            else
            {
                Texture2D texture2D = DownloadHandlerTexture.GetContent(uwr);
                resolve(texture2D);
            }
        }
    }

    public void DestroyAsset(Object asset)
    {
        Assert.IsNotNull(asset);
        
        Object.Destroy(asset);
    }
}
