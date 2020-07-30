using RSG;
using System.Collections.Generic;

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
    private readonly Dictionary<string, string> _lowerDict = new Dictionary<string, string>();

    private ResourceManager()
    {
    }

    private bool _useBundle;

    public void Initialize(bool useAssetBundle)
    {
        _useBundle = useAssetBundle;
    }

    public IPromise<T> GetAssetSync<T>(string path, object owner) where T : Object
    {
        if (_useBundle)
        {
            string assetPath = GetAssetBundleName(path);
            string assetName = GetAssetName(path);

            return GetAssetFromAssetBundleSync<T>(assetPath, assetName, owner);
        }
        else
        {
#if UNITY_EDITOR
            return ResourceManagerAsset.Instance.GetAssetAssetSync<T>(AssetEditorLoader.Instance, path, owner);
#else
            return Promise<T>.Rejected(new Exception("None Editor can't load Asset"));
#endif
        } 
    }

    public IPromise<T> GetAsset<T>(string path, object owner) where T : Object
    {
        if (_useBundle)
        {
            string assetPath = GetAssetBundleName(path);
            string assetName = GetAssetName(path);

            return GetAssetFromAssetBundle<T>(assetPath, assetName, owner);
        }
        else
        {
#if UNITY_EDITOR
            return ResourceManagerAsset.Instance.GetAssetAsset<T>(AssetEditorLoader.Instance, path, owner);
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
            ResourceManagerAsset.Instance.DestroyAsset(path, owner);
#else
            return Promise<T>.Rejected(new Exception("None Editor can't destroy Asset"));
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

    private string GetAssetName(string str)
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
