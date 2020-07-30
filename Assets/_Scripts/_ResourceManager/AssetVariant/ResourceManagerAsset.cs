using System;
using System.Collections;
using System.Collections.Generic;
using RSG;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using System.Runtime.InteropServices;
using Object = UnityEngine.Object;

public class ResourceManagerAsset
{
    public const int DEFAULT_LIST_SIZE = 4;

    private static ResourceManagerAsset _instance;

    public static ResourceManagerAsset Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourceManagerAsset();
            }

            return _instance;
        }
    }

    private ResourceManagerAsset()
    { }
    
    class AssetWrapper
    {
        class RefData
        {
            public object owner;
            public List<int> internalRefList = new List<int>(DEFAULT_LIST_SIZE);
        }

        public string path;
        public IResourceLoader loader;
        public Object asset; // TODO:: Maybe WeakReference<Object>

        public bool loadDone;
        public List<Action> loadDoneAction;

        private List<RefData> _referenceList = new List<RefData>(DEFAULT_LIST_SIZE);

        private RefData FindRefData(object owner)
        {
            for (int i = 0, length = _referenceList.Count; i < length; ++i)
            {
                RefData refData = _referenceList[i];

                if (refData.owner == owner)
                {
                    return refData;
                }
            }

            return null;
        }

        public void DestroyAllReference()
        {
            _referenceList.Clear();
        }

        public void DestroyReference(object owner)
        {
            RefData refData = FindRefData(owner);
            Assert.IsNotNull(refData);

            Assert.IsTrue(refData.internalRefList.Count > 0);

            refData.internalRefList.RemoveAt(0);    // remove first internal id
            if (refData.internalRefList.Count == 0)
            {
                bool removed = _referenceList.Remove(refData);
                Assert.IsTrue(removed, string.Format("remove {0} is not exist", owner));
            }
        }

        public void AddReference(object owner, int id)
        {
            RefData refData = FindRefData(owner);

            if (refData == null)
            {
                refData = new RefData { owner = owner };
                refData.internalRefList.Add(id);

                _referenceList.Add(refData);
            }
            else
            {
                Assert.IsNotNull(refData.internalRefList);
                Assert.IsFalse(refData.internalRefList.Contains(id));

                refData.internalRefList.Add(id);
            }
        }

        public bool IsAllDestroyed()
        {
            return _referenceList.Count == 0;
        }

        public bool IsContainTarget(object owner, int id)
        {
            RefData refData = FindRefData(owner);

            return refData != null && refData.internalRefList.Contains(id);
        }

        public int GetReferenceCount()
        {
            int refCount = 0;
            for (int i = 0, length = _referenceList.Count; i < length; ++i)
            {
                refCount += _referenceList[i].internalRefList.Count;
            }

            return refCount;
        }
    }

    private Dictionary<string, AssetWrapper> _assetWrappers = new Dictionary<string, AssetWrapper>();

    public void DestroyAsset(string path, object owner)
    {
        AssetWrapper wrapper;
        if (_assetWrappers.TryGetValue(path, out wrapper))
        {
            wrapper.DestroyReference(owner);

            if (wrapper.IsAllDestroyed())
            {
                DestroyAllAsset(path);
            }
            else
            {
                // nothing
            }
        }
        else
        {
            throw new Exception(string.Format("DestroyAsset path: {0} owner: {1}", path, owner));
        }
    }

    private void DestroyAllAsset(string path)
    {
        AssetWrapper wrapper = null;
        if (_assetWrappers.TryGetValue(path, out wrapper))
        {
            if (wrapper.loadDone)
            {
                Object asset = wrapper.asset;
                
                wrapper.loader.DestroyAsset(asset);
                _assetWrappers.Remove(path);
            }
            else
            {
                wrapper.DestroyAllReference();
            }
        }
        else
        {
            throw new Exception("Asset not found");
        }
    }

    public IPromise<T> GetAssetAssetSync<T>(IResourceLoader loader, string path, object owner) where T : Object
    {
        AssetWrapper assetWrapper;

        int id = GetNextIDAsset();

        if (_assetWrappers.TryGetValue(path, out assetWrapper))
        {
            assetWrapper.AddReference(owner, id);

            if (assetWrapper.loadDone)
            {
                T asset = assetWrapper.asset as T;
                if (asset)
                {
                    return Promise<T>.Resolved(asset);
                }
                else
                {
                    return Promise<T>.Rejected(new Exception($"asset == null. path: {path}"));
                }
            }
            else
            {
                throw new Exception($"GetAssetAssetSync got async loading. path: {path}");
            }
        }
        else
        {
            var promise = new Promise<T>();

            assetWrapper = new AssetWrapper()
            {
                path = path,
                loader = loader,
                asset = null,
                loadDone = false,
                loadDoneAction = new List<Action>(DEFAULT_LIST_SIZE),
            };
            assetWrapper.AddReference(owner, id);
            _assetWrappers.Add(path, assetWrapper);

            loader.BrandNewLoadAssetSync<T>(path)
                .Then(asset =>
                {
                    assetWrapper.asset = asset;
                    assetWrapper.loadDone = true;
                    
                    promise.Resolve(asset as T);
                })
                .Catch(ex => promise.Reject(ex));

            return promise;
        }
    }
    
    public IPromise<T> GetAssetAsset<T>(IResourceLoader loader, string path, object owner) where T : Object
    {
        AssetWrapper assetWrapper;

        int id = GetNextIDAsset();
        if (_assetWrappers.TryGetValue(path, out assetWrapper))
        {
            assetWrapper.AddReference(owner, id);

            if (assetWrapper.loadDone)
            {
                return RetriveAsset<T>(assetWrapper, path, owner, id);
            }
            else
            {
                return DelayRetriveAsset<T>(assetWrapper, path, owner, id);
            }
        }
        else
        {
            return BrandNewLoadAsset<T>(loader, path, owner, id);
        }
    }

    private IPromise<T> RetriveAsset<T>(AssetWrapper assetWrapper, string path, object owner, int id) where T : Object
    {
        var promise = new Promise<T>();

        var asset = assetWrapper.asset;
        if (asset)
        {
            if (IsTargetDestroyed(assetWrapper, owner, id))
            {
                promise.Reject(new Exception($"target is destroy: {path}"));
            }
            else
            {
                T result = asset as T;
                if (result)
                {
                    promise.Resolve(result);
                }
                else
                {
                    promise.Reject(new Exception($"asset == null: {path}"));
                }
            }
        }
        else
        {
            promise.Reject(new Exception($"asset == null: {path}"));
        }

        return promise;
    }

    private void AddLoadDoneAction<T>(Promise<T> promise, AssetWrapper assetWrapper, string path, object owner, int id) where T : Object
    {
        Action loadDoneAction = new Action(() =>
        {
            T asset = assetWrapper.asset as T;

            if (assetWrapper.IsAllDestroyed())
            {
                promise.Reject(new Exception($"All destroy: {path}"));
            }
            else if (IsTargetDestroyed(assetWrapper, owner, id))
            {
                promise.Reject(new Exception($"owner destroyed: {path}"));
            }
            else if (!assetWrapper.IsContainTarget(owner, id))
            {
                promise.Reject(new Exception($"not contain target {path}"));
            }
            else
            {
                T result = asset;
                if (result)
                {
                    promise.Resolve(result);
                }
                else
                {
                    promise.Reject(new Exception($"asset == null: {path}"));
                }
            }
        });

        assetWrapper.loadDoneAction.Add(loadDoneAction);
    }

    private IPromise<T> DelayRetriveAsset<T>(AssetWrapper assetWrapper, string path, object owner, int id) where T : Object
    {
        var promise = new Promise<T>();

       AddLoadDoneAction(promise, assetWrapper, path, owner, id);

        return promise;
    }


    private IPromise<T> BrandNewLoadAsset<T>(IResourceLoader loader, string path, object owner, int id) where T : UnityEngine.Object
    {
        var promise = new Promise<T>();

        var assetWrapper = new AssetWrapper()
        {
            path = path,
            loader = loader,
            asset = null,
            loadDone = false,
            loadDoneAction = new List<Action>(DEFAULT_LIST_SIZE),
        };
        assetWrapper.AddReference(owner, id);
        AddLoadDoneAction(promise, assetWrapper, path, owner, id);

        _assetWrappers.Add(path, assetWrapper);

        loader.BrandNewLoadAsset<T>(path)
            .Then(obj =>
            {
                assetWrapper.asset = obj;

                OnAssetLoadFinish(assetWrapper);
            })
            .Catch(ex =>
            {
                assetWrapper.asset = null;
                
                OnAssetLoadFinish(assetWrapper);
            });
        

        return promise;
    }

    private void OnAssetLoadFinish(AssetWrapper wrapper)
    {
        wrapper.loadDone = true;

        if (wrapper.loadDoneAction != null)
        {
            for (int i = 0, length = wrapper.loadDoneAction.Count; i < length; ++i)
            {
                wrapper.loadDoneAction[i]();
            }
            wrapper.loadDoneAction = null;
        }

        if (wrapper.IsAllDestroyed())
        {
            Object asset = wrapper.asset;
            if (asset != null)
            {
                wrapper.loader.DestroyAsset(asset);
            }
            else
            {
                // nothing
            }

            string path = wrapper.path;
            _assetWrappers.Remove(path);
        }
    }

    private static bool IsTargetDestroyed(AssetWrapper wrapper, object owner, int id)
    {
        return !wrapper.IsContainTarget(owner, id);
    }

    private static int _idAsset= 0;

    private static int GetNextIDAsset()
    {
        _idAsset = _idAsset + 1;

        return _idAsset;
    }
}
