using System.Collections;
using System.Collections.Generic;
using RSG;
using UnityEngine;
using UnityEngine.Assertions;

public class PoolCacheManager
{
    private static PoolCacheManager _instance;

    public static PoolCacheManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PoolCacheManager();
            }

            return _instance;
        }
    }

    private const string PATH_FINDING_TEMPLATE = "Assets/Res/Template/Pathfinding.prefab";
    private GameObject _pathfindingPrefab;
    
    private Dictionary<GameObject, EntityPathfinding> _pathfindingCacheDict = new Dictionary<GameObject, EntityPathfinding>(32);
    
    private PoolCacheManager()
    {}

    public IPromise WarmPathfinding(int poolCount)
    {
        var promise = new Promise();
        
        ResourceManager.Instance.GetAsset<GameObject>(PATH_FINDING_TEMPLATE, this)
            .Then(asset =>
            {
                _pathfindingPrefab = asset;
                
                PoolManager.Instance.WarmPool(asset, poolCount);
                
                promise.Resolve();
            })
            .Catch(ex =>
            {
                Debug.LogException(ex);

                promise.Reject(ex);
            });

        return promise;
    }

    public EntityPathfinding SpawnEntityPathfinding()
    {
        Assert.IsNotNull(_pathfindingPrefab);

        GameObject go = PoolManager.Instance.SpawnObject(_pathfindingPrefab);

        EntityPathfinding pathfinding;
        if (_pathfindingCacheDict.TryGetValue(go, out pathfinding))
        {
            return pathfinding;
        }
        else
        {
            pathfinding = go.GetComponent<EntityPathfinding>();
            _pathfindingCacheDict.Add(go, pathfinding);

            return pathfinding;
        }
    }

    public void ReturnEntityPathfinding(EntityPathfinding pathfinding)
    {
        Assert.IsNotNull(pathfinding);
        
        PoolManager.Instance.ReleaseObject(pathfinding.gameObject);
    }
}
