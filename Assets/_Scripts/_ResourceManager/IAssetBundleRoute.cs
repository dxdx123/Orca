using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class IAssetBundleRoute
{
    public abstract string LookupAssetBundleName(string assetPath);
    
    public abstract string LookupAssetBundlePath(string assetBundleName);
}

public class AssetBundleRoute : IAssetBundleRoute
{
    private readonly Dictionary<string, string> _pathDict = new Dictionary<string, string>(128);
    private readonly Dictionary<string, string> _assetBundleNameDict = new Dictionary<string, string>(128);

    private AssetBundleVariantData _assetBundleVariantData;
    public AssetBundleRoute(AssetBundleRouteData assetBundleRouteData, AssetBundleVariantData assetBundleVariantData)
    {
        InitializePath2AssetBundle(assetBundleRouteData.list);

        _assetBundleVariantData = assetBundleVariantData;
        assetBundleVariantData.Initialize();
    }

    private void InitializePath2AssetBundle(List<AssetBundleRef> list)
    {
        for (int i = 0, length = list.Count; i < length; ++i)
        {
            var refData = list[i];

            string path = refData.path;
            string assetBundlePath = refData.assetBundlePath;
            
            _assetBundleNameDict.Add(path, assetBundlePath);
        }
    }

    public override string LookupAssetBundleName(string assetPath)
    {
        Assert.IsNotNull(assetPath);

        string newName;
        if(_assetBundleNameDict.TryGetValue(assetPath, out newName))
        {
            return newName;
        }
        else
        {
            throw new Exception($"Unknown locate assetBundleName: {assetPath}");
        }
    }
    
    public override string LookupAssetBundlePath(string assetBundleName)
    {
        Assert.IsNotNull(assetBundleName);

        var variantAssetName = _assetBundleVariantData.GetAssetBundleVariantPath(assetBundleName);
        
        if (!string.IsNullOrEmpty(variantAssetName))
        {
            return GetAssetBundlePath(variantAssetName);
        }
        else
        {
            return GetAssetBundlePath(assetBundleName);
        }
    }

    private string GetAssetBundlePath(string assetBundleName)
    {
        string fullPath;
        if (_pathDict.TryGetValue(assetBundleName, out fullPath))
        {
            return fullPath;
        }
        else
        {
            fullPath = $"{Application.streamingAssetsPath}/AssetBundles/{assetBundleName}";
            _pathDict.Add(assetBundleName, fullPath);

            return fullPath;
        }
    }
}
