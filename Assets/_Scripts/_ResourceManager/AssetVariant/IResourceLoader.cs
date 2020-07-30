using System.Collections;
using System.Collections.Generic;
using RSG;
using UnityEngine;

public interface IResourceLoader
{
    IPromise<Object> BrandNewLoadAsset<T>(string path) where T : Object;
    IPromise<Object> BrandNewLoadAssetSync<T>(string path) where T : Object;
    
    void DestroyAsset(Object asset);
}
