using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRefIncreaseEvent : GameEvent
{
    public string assetBundleName;
    public object owner;

    public override string ToString()
    {
        return $"ResourceRefIncreaseEvent assetBundleName: {assetBundleName}, owner: {owner}";
    }
}

public class ResourceRefDecreaseEvent : GameEvent
{
    public string assetBundleName;
    public object owner;

    public override string ToString()
    {
        return $"ResourceRefDecreaseEvent assetBundleName: {assetBundleName}, owner: {owner}";
    }
}
