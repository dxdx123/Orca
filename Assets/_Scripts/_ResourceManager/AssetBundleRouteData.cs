using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleRouteData : ScriptableObject
{
    public List<AssetBundleRef> list;
}

[System.Serializable]
public class AssetBundleRef
{
    public string path;

    public string assetBundlePath;
}
