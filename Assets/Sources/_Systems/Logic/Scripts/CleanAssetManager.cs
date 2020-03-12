using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CleanAssetManager
{
    private static CleanAssetManager _instance;

    public static CleanAssetManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CleanAssetManager();
            }

            return _instance;
        }
    }
    
    private CleanAssetManager()
    {}

    private Dictionary<GameEntity, List<Action>> _dict = new Dictionary<GameEntity, List<Action>>();
    
    public void RegisterCleanAssetActions(GameEntity entity, Action action)
    {
        Assert.IsNotNull(entity);
        Assert.IsNotNull(action);

        List<Action> list;
        if (_dict.TryGetValue(entity, out list))
        {
            list.Add(action);
        }
        else
        {
            list = new List<Action>(1){action};
            _dict.Add(entity, list);
            
            entity.Retain(this);
        }
    }

    public void CleanAsset(GameEntity entity)
    {
        Assert.IsNotNull(entity);

        List<Action> list;
        if (_dict.TryGetValue(entity, out list))
        {
            foreach (var action in list)
            {
                action();
            }

            _dict.Remove(entity);
            entity.Release(this);
        }
        else
        {
            // nothing
        }
    }
}
