using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AsestBundleVariantStategy
{
    None,
    
    SimpleVariant,
}

public class AssetBundleVariantData : ScriptableObject
{
    public List<AbPath2IdWrapper> abPathIdList;
    public List<Id2AbPathWrapper> idAbPathList;

    private Dictionary<string, AsestBundleVariantStategy> _abPath2StrategyDict = null;
    private Dictionary<string, string> _abPath2IdDict;
    private Dictionary<Tuple<string, string>, string> _id2AbPathDict;
    
    public void Initialize()
    {
        _abPath2IdDict = new Dictionary<string, string>(abPathIdList.Count);
        for (int i = 0, length = abPathIdList.Count; i < length; ++i)
        {
            var wrapper = abPathIdList[i];
            
            _abPath2IdDict.Add(wrapper.abPath, wrapper.id);
        }
        
        _id2AbPathDict = new Dictionary<Tuple<string, string>, string>(idAbPathList.Count);
        _abPath2StrategyDict = new Dictionary<string, AsestBundleVariantStategy>(idAbPathList.Count);
        for (int i = 0, length = idAbPathList.Count; i < length; ++i)
        {
            var wrapper = idAbPathList[i];

            string abPath = wrapper.abPath;
            
            var tuple = Tuple.Create(wrapper.id, wrapper.variantName);
            _id2AbPathDict.Add(tuple, abPath);
            
            _abPath2StrategyDict.Add(abPath, wrapper.strategy);
        }
    }

    public string GetAssetBundleVariantPath(string assetName)
    {
        string id;

        if (_abPath2IdDict.TryGetValue(assetName, out id))
        {
            string variantName = GetVariantName();

            var tuple = Tuple.Create(id, variantName);

            string newAssetName;
            if (_id2AbPathDict.TryGetValue(tuple, out newAssetName))
            {
                return newAssetName;
            }
            else
            {
                throw new Exception($"!!! Not found {tuple}");
            }
        }
        else
        {
            return null;
        }
    }

    private string GetVariantName()
    {
        // TODO:: implement all the variant logic
        return "B";
    }
}

[Serializable]
public class AbPath2IdWrapper
{
    public string abPath;
    public string id;
}

[Serializable]
public class Id2AbPathWrapper
{
    public string id;
    public string variantName;
    public AsestBundleVariantStategy strategy;

    public string abPath;
}
