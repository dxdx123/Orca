using System;
using RSG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

public class ResourceManager
{
    private static ResourceManager _instance;

    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourceManager();
            }

            return _instance;
        }
    }

    private readonly Dictionary<string, string> _assetPathDict = new Dictionary<string, string>();

    private readonly Dictionary<object, List<string>> _ownerDict = new Dictionary<object, List<string>>();
    
    private ResourceManager()
    {
    }

    private bool _useBundle;

    public void Initialize(bool useAssetBundle)
    {
        _useBundle = useAssetBundle;
    }

    public T GetAssetSync<T>(string path, object owner) where T : Object
    {
        Assert.IsNotNull(path);
        Assert.IsNotNull(owner);
        
        AddReference(path, owner);
        
        T result = null;

        GetAssetSyncInternal<T>(path, owner)
            .Then(asset =>
            {
                result = asset;
            })
            .Catch(ex =>
            {
                Debug.LogException(ex);
                result = null;
            });

        return result;
    }

    private IPromise<T> GetAssetSyncInternal<T>(string path, object owner) where T : Object
    {
        if (_useBundle)
        {
            string assetPath = GetAssetBundleName(path);
            string assetName = path;

            return GetAssetFromAssetBundleSync<T>(assetPath, assetName, owner);
        }
        else
        {
#if UNITY_EDITOR
            return ResourceManagerAsset.Instance.GetAssetAssetSync<T>(AssetEditorLoader.Instance, path, owner);
#else
            return Promise<T>.Rejected(new System.Exception("None Editor can't load Asset"));
#endif
        }
    }

    public IPromise<T> GetAsset<T>(string path, object owner) where T : Object
    {
        Assert.IsNotNull(path);
        Assert.IsNotNull(owner);
        
        AddReference(path, owner);
        
        if (_useBundle)
        {
            string assetPath = GetAssetBundleName(path);
            string assetName = path;

            return GetAssetFromAssetBundle<T>(assetPath, assetName, owner);
        }
        else
        {
#if UNITY_EDITOR
            return ResourceManagerAsset.Instance.GetAssetAsset<T>(AssetEditorLoader.Instance, path, owner);
#else
            return Promise<T>.Rejected(new System.Exception("None Editor can't load Asset"));
#endif
        }
    }

    public void DestroyAsset(string path, object owner)
    {
        Assert.IsNotNull(path);
        Assert.IsNotNull(owner);
        
        DestroyAssetInternal(path, owner, true);
    }

    public void DestroyAllAsset(object owner)
    {
        Assert.IsNotNull(owner);

        List<string> list;

        if (_ownerDict.TryGetValue(owner, out list))
        {
            foreach (var path in list)
            {
                DestroyAssetInternal(path, owner, false);
            }
            
            _ownerDict.Remove(owner);
        }
        else
        {
            // nothing
        }
    }

    private void DestroyAssetInternal(string path, object owner, bool removeReference)
    {
        if (removeReference)
        {
            RemoveReference(path, owner);
        }
        else
        {
            // nothing
        }
        
        if (_useBundle)
        {
            string assetBundleName = GetAssetBundleName(path);

            ResourceManagerAssetBundle.Instance.DestroyAssetBundle(assetBundleName, owner);
        }
        else
        {
#if UNITY_EDITOR
            ResourceManagerAsset.Instance.DestroyAsset(path, owner);
#else
            throw new System.Exception("None Editor can't destroy Asset");
#endif
        }
    }

    private IPromise<T> GetAssetFromAssetBundleSync<T>(string assetPath, string assetName, object owner) where T : Object
    {
        return ResourceManagerAssetBundle.Instance.GetAssetBundleAssetSync<T>(assetPath, assetName, owner);
    }

    private IPromise<T> GetAssetFromAssetBundle<T>(string assetPath, string assetName, object owner) where T : Object
    {
        return ResourceManagerAssetBundle.Instance.GetAssetBundleAsset<T>(assetPath, assetName, owner);
    }

    private string GetAssetBundleName(string path)
    {
        string assetPath;
        if (_assetPathDict.TryGetValue(path, out assetPath))
        {
            return assetPath;
        }
        else
        {
            assetPath = path.ToLower();

            _assetPathDict.Add(path, assetPath);

            return assetPath;
        }
    }
    
    // =============================== helper ===============================
    private void AddReference(string path, object owner)
    {
        List<string> list;
        if (_ownerDict.TryGetValue(owner, out list))
        {
            list.Add(path);
        }
        else
        {
            list = new List<string>(1)
            {
                path
            };
            
            _ownerDict.Add(owner, list);
        }
    }

    private void RemoveReference(string path, object owner)
    {
        List<string> list;

        if (_ownerDict.TryGetValue(owner, out list))
        {
            int index = -1;

            for (int i = 0; i < list.Count; ++i)
            {
                if (path == list[i])
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                list.RemoveAt(index);

                if (list.Count > 0)
                {
                    // still got ref
                }
                else
                {
                    _ownerDict.Remove(owner);
                }
            }
            else
            {
                throw new Exception($"RemoveReference not found path, path: {path}, owner: {owner}");
            }
        }
        else
        {
            throw new Exception($"RemoveReference not found list, path: {path}, owner: {owner}");
        }
    }
}
