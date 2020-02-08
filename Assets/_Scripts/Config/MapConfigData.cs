using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu]
public class MapConfigData : ScriptableObject
{
    public List<MapConfig> list;

    private Dictionary<string, MapConfig> _dict = new Dictionary<string, MapConfig>();
    
    public void Initialize()
    {
        foreach (var mapConfig in list)
        {
            string mapName = mapConfig.mapName;
            
            _dict.Add(mapName, mapConfig);
        }
    }

    public MapConfig GetMapConfig(string mapName)
    {
        MapConfig mapConfig;
        if (_dict.TryGetValue(mapName, out mapConfig))
        {
            return mapConfig;
        }
        else
        {
            throw new Exception($"Invalid {mapConfig}");
        }
    }
}

[Serializable]
public class MapConfig
{
    public string mapName;
    public int width;
    public int height;
}
