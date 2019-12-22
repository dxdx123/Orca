using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AssetBundleVariantRef
{
    public string abPath;
    public List<string> variants;
}

public class AssetBundleVariantData : ScriptableObject
{
    public List<AssetBundleVariantRef> list;

    private bool _initialize;
    private Dictionary<string, Dictionary<string, string>> _dict;

    public Dictionary<string, Dictionary<string, string>> GetDictionary()
    {
        if (!_initialize)
        {
            Initialize();
            _initialize = true;
        }

        return _dict;
    }

    private void Initialize()
    {
        int length = list.Count;
        
        Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>(length);

        for (int i = 0; i < length; ++i)
        {
            string abPath = list[i].abPath;
            List<string> varList = list[i].variants;
            
            dict.Add(abPath, varList);
        }

        _dict = GenerateInternal(dict);
    }
    
    
    private Dictionary<string, Dictionary<string, string>> GenerateInternal(Dictionary<string, List<string>> dict)
    {
        Dictionary<string, Dictionary<string, string>> abPath2FullDict = new Dictionary<string, Dictionary<string, string>>();
            
        foreach (var item in dict)
        {
            string abPath = item.Key;
            List<string> varList = item.Value;
                
            Dictionary<string, string> variantToFullPathDict = GenerateVariantToFullPathDict(abPath, varList);

            abPath2FullDict[abPath] = variantToFullPathDict;
        }
            
        Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();

        foreach (var item in abPath2FullDict)
        {
            string abPath = item.Key;
            var innerDict = item.Value;

            Dictionary<string, Dictionary<string, string>> expandDict = ExpandVariantDict(abPath, innerDict);

            foreach (var innerItem in expandDict)
            {
                result[innerItem.Key] = innerItem.Value;
            }
        }

        return result;
    }

    private Dictionary<string, Dictionary<string, string>> ExpandVariantDict(string abPath, Dictionary<string, string> innerDict)
    {
        Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();

        foreach (var item in innerDict)
        {
            string variantName = item.Key;

            string key = abPath + "." + variantName;

            dict[key] = innerDict;
        }
            
        return dict;
    }
    
    private Dictionary<string, string> GenerateVariantToFullPathDict(string abPath, List<string> varList)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
            
        foreach (var variantName in varList)
        {
            string fullPath = abPath + "." + variantName;
            dict[variantName] = fullPath;
        }

        return dict;
    }

}
