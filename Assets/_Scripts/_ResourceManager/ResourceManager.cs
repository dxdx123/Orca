using System;
using System.Collections;
using System.Collections.Generic;
using RSG;
using UniRx;
using UnityEditor;
using UnityEngine;
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

    private readonly Dictionary<string, string> _assetBundleDict = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _lowerDict = new Dictionary<string, string>();

    private ResourceManager()
    {
    }

    private bool _useBundle = false;

    public void Initialize(bool useAssetBundle)
    {
        _useBundle = useAssetBundle;
    }

    public IPromise<T> GetAsset<T>(string path, object owner) where T : Object
    {
        if (_useBundle)
        {
            string assetBundleName = GetAssetBundleName(path);
            string assetName = GetAssetName(path);

            return GetAssetFromAssetBundle<T>(assetBundleName, assetName, owner);
        }
        else
        {
#if UNITY_EDITOR
            return ResourceManagerEditor.Instance.GetAssetEditor<T>(path, owner);
#else
            return Promise<T>.Rejected(new Exception("None Editor can't load Asset"));
#endif
        }
    }

    public void DestroyAsset(string path, object owner)
    {
        if (_useBundle)
        {
            string assetBundleName = GetAssetBundleName(path);

            ResourceManagerAssetBundle.Instance.DestroyAssetBundle(assetBundleName, owner);
        }
        else
        {
#if UNITY_EDITOR
            ResourceManagerEditor.Instance.DestroyAssetEditor(path, owner);
#endif
        }
    }

    private IPromise<T> GetAssetFromAssetBundle<T>(string assetBundleName, string assetName, object owner) where T : Object
    {
        return ResourceManagerAssetBundle.Instance.GetAssetBundleAsset<T>(assetBundleName, assetName, owner);
    }

    public string GetAssetBundleName(string path)
    {
        string assetBundleName;
        if (_assetBundleDict.TryGetValue(path, out assetBundleName))
        {
            return assetBundleName;
        }
        else
        {
            assetBundleName = path.ToLower();

            _assetBundleDict.Add(path, assetBundleName);

            return assetBundleName;
        }
    }

    public string GetAssetName(string str)
    {
        string lowerString;

        if (_lowerDict.TryGetValue(str, out lowerString))
        {
            return lowerString;
        }
        else
        {
            lowerString = str.ToLower();

            _lowerDict.Add(str, lowerString);

            return lowerString;
        }
    }
}
