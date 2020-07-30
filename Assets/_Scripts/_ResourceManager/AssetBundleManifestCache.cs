using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AssetBundleManifestCache
{
    private AssetBundleManifest _manifest;

    private readonly Dictionary<string, string[]> _depDict = new Dictionary<string, string[]>(128);

    public AssetBundleManifestCache(AssetBundleManifest manifest)
    {
        Assert.IsNotNull(manifest);

        _manifest = manifest;
    }

    public string[] GetAllDependencies(string assetBundleName)
    {
        Assert.IsNotNull(assetBundleName);

        string[] deps;

        if(_depDict.TryGetValue(assetBundleName, out deps))
        {
            return deps;
        }
        else
        {
            deps = _manifest.GetAllDependencies(assetBundleName);
            _depDict.Add(assetBundleName, deps);
            return deps;
        }
    }
}
