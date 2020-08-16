using System.Collections;
using System.Collections.Generic;
using RSG;
using UnityEngine;

public class AssetPoolManager
{
    private static AssetPoolManager _instance;

    public static AssetPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AssetPoolManager();
            }

            return _instance;
        }
    }

    private GameObject _characterAsset;
    private GameObject _puppyAsset;
    private GameObject _enemyAsset;
    private GameObject _effectAsset;
    
    private GameObject _mapAsset;
    
    public AssetPoolManager()
    {
    }

    public IPromise Initialize(int characterSize, int puppySize, int enemySize, int effectSize, int mapSize)
    {
        var promise = new Promise();

        InitializeCharacter(characterSize);
        InitializePuppy(puppySize);
        InitializeEnemy(enemySize);
        InitializeEffect(effectSize);
        InitializeMap(mapSize);
        
        promise.Resolve();

        return promise;
    }

    private void InitializeMap(int mapSize)
    {
        // _mapAsset = Resources.Load<GameObject>("Map_Template");
        _mapAsset = ResourceManager.Instance.GetAssetSync<GameObject>(
            "Assets/Res/Sync/Map_Template.prefab", this);
        
        PoolManager.Instance.WarmPool(_mapAsset, mapSize);
    }

    private void InitializeCharacter(int characterSize)
    {
        // _characterAsset = Resources.Load<GameObject>("Character_Template");
        _characterAsset = ResourceManager.Instance.GetAssetSync<GameObject>(
            "Assets/Res/Sync/Character_Template.prefab", this);
        
        PoolManager.Instance.WarmPool(_characterAsset, characterSize);
    }
    
    private void InitializePuppy(int puppySize)
    {
        // _puppyAsset = Resources.Load<GameObject>("Puppy_Template");
        _puppyAsset = ResourceManager.Instance.GetAssetSync<GameObject>(
            "Assets/Res/Sync/Puppy_Template.prefab", this);
        
        PoolManager.Instance.WarmPool(_puppyAsset, puppySize);
    }

    private void InitializeEnemy(int enemySize)
    {
        // _enemyAsset = Resources.Load<GameObject>("Enemy_Template");
        _enemyAsset = ResourceManager.Instance.GetAssetSync<GameObject>(
            "Assets/Res/Sync/Enemy_Template.prefab", this);
        
        PoolManager.Instance.WarmPool(_enemyAsset, enemySize);
    }

    private void InitializeEffect(int effectSize)
    {
        // _effectAsset = Resources.Load<GameObject>("Effect_Template");
        _effectAsset = ResourceManager.Instance.GetAssetSync<GameObject>(
            "Assets/Res/Sync/Effect_Template.prefab", this);
        
        PoolManager.Instance.WarmPool(_effectAsset, effectSize);
    }

    public GameObject SpawnCharacter()
    {
        return PoolManager.Instance.SpawnObject(_characterAsset);
    }

    public GameObject SpawnPuppy()
    {
        return PoolManager.Instance.SpawnObject(_puppyAsset);
    }
    
    public GameObject SpawnEnemy()
    {
        return PoolManager.Instance.SpawnObject(_enemyAsset);
    }

    public GameObject SpawnEffect()
    {
        return PoolManager.Instance.SpawnObject(_effectAsset);
    }

    public GameObject SpawnMap(Vector3 position, Quaternion rotation)
    {
        return PoolManager.Instance.SpawnObject(_mapAsset, position, rotation);
    }
    
    public void DestroyInstance(GameObject instance)
    {
        PoolManager.Instance.ReleaseObject(instance);
    }
}
