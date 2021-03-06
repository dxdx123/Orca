using RSG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.Assertions;

using Object = UnityEngine.Object;

public class ResourceManagerAssetBundle
{
    private static ResourceManagerAssetBundle _instance;

    public static ResourceManagerAssetBundle Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new ResourceManagerAssetBundle();
            }

            return _instance;
        }
    }

    private AssetBundleManifestCache _manifest;
    private IAssetBundleRoute _route;

    private static int assetBundleId;

    private const int DefaultListSize = 4;

    class AssetBundleWrapper
    {
        public class RefData
        {
            public object owner;
            public List<int> internalRefList = new List<int>(DefaultListSize);
        }

        public string path;
        public AssetBundle assetBundle;

        public Promise<AssetBundle> promise;
        public Dictionary<string, Promise> loadingAssetSet;

        private List<RefData> _referenceList = new List<RefData>(DefaultListSize);
        
        public Dictionary<string, WeakReference<Object>> assetDict = new Dictionary<string, WeakReference<Object>>();

        public bool IsLoadingAsset()
        {
            return loadingAssetSet.Count > 0;
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

            RaiseIncreaseEvent(path, owner);
        }

        public bool IsContainTarget(object owner, int id)
        {
            RefData refData = FindRefData(owner);

            return refData != null && refData.internalRefList.Contains(id);
        }

        public void RemoveReference(object owner)
        {
            RefData refData = FindRefData(owner);
            if (refData == null)
            {
                Debug.LogError($"Destroy NO refs assetBundle: {path}, owner: {owner}");                
                return;
            }

            Assert.IsTrue(refData.internalRefList.Count > 0);

            refData.internalRefList.RemoveAt(0);    // remove first internal id
            if (refData.internalRefList.Count == 0)
            {
                bool removed = _referenceList.Remove(refData);
                Assert.IsTrue(removed, $"remove {owner} is not exist");
            }

            RaiseDecreaseEvent(path, owner);
        }

        public List<RefData> GetReferenceList()
        {
            return _referenceList;
        }

        public int GetReferenceCount()
        {
            return _referenceList.Count;
        }

        public void Destroy()
        {
            bool loadDone = (promise.CurState == PromiseState.Rejected || promise.CurState == PromiseState.Resolved);
            Assert.AreEqual(true, loadDone);

            Assert.IsNotNull(assetBundle);
            Assert.AreEqual(0, _referenceList.Count);

            assetBundle.Unload(true);
        }

        private RefData FindRefData(object owner)
        {
            for (int i = 0, length = _referenceList.Count; i < length; ++i)
            {
                RefData refData = _referenceList[i];

                if (refData.owner.Equals(owner))
                {
                    return refData;
                }
            }

            return null;
        }
    }

    private readonly Dictionary<string, AssetBundleWrapper> _assetBundleWrappers = new Dictionary<string, AssetBundleWrapper>(DefaultListSize);

    private static readonly ResourceRefIncreaseEvent RefIncreaseEvent = new ResourceRefIncreaseEvent();
    private static readonly ResourceRefDecreaseEvent RefDecreaseEvent = new ResourceRefDecreaseEvent();
    
    private ResourceManagerAssetBundle() { }

    public void Initialize(AssetBundleManifest manifest, IAssetBundleRoute route)
    {
        Assert.IsNotNull(manifest);
        Assert.IsNotNull(route);

        _manifest = new AssetBundleManifestCache(manifest);
        _route = route;
    }

    public AssetBundle GetAssetBundleSync(string assetBundleName, object owner)
    {
        Assert.IsNotNull(assetBundleName);
        Assert.IsNotNull(owner);

        AssetBundle assetBundle = null;

        GetAssetBundleSyncInternal(assetBundleName, owner)
            .Then(ab => { assetBundle = ab; })
            .Catch(ex =>
            {
                Debug.LogException(ex);
                assetBundle = null;
            });

        return assetBundle;
    }

    private IPromise<AssetBundle> GetAssetBundleSyncInternal(string assetBundleName, object owner)
    {
        var dependencies = _manifest.GetAllDependencies(assetBundleName);
        int length = dependencies.Length;

        if (length == 0)
        {
            return LoadAssetBundleSyncInternal(assetBundleName, owner);
        }
        else
        {
            IPromise<AssetBundle>[] promises = new IPromise<AssetBundle>[length + 1];

            // dependency
            for (int i = 0; i < length; ++i)
            {
                string depBundleName = dependencies[i];
                promises[i] = LoadAssetBundleSyncInternal(depBundleName, owner);
            }

            // self
            promises[length] = LoadAssetBundleSyncInternal(assetBundleName, owner);

            var promise = new Promise<AssetBundle>();

            Promise<AssetBundle>.All(promises)
                .Then(assetBundles =>
                {
                    var assetBundle = GetIndexAssetBundle(assetBundles, length);
                    promise.Resolve(assetBundle);
                })
                .Catch(ex => { promise.Reject(ex); });

            return promise;
        }
    }

    private IPromise<AssetBundle> LoadAssetBundleSyncInternal(string assetBundleName, object owner)
    {
        int id = GetNextIdAssetBundle();
        
        AssetBundleWrapper wrapper;
        if (_assetBundleWrappers.TryGetValue(assetBundleName, out wrapper))
        {
            if (wrapper.assetBundle == null)
            {
                throw new Exception($"Sync Load AssetBundle {assetBundleName} but got Async Bundle loading...");
            }
            
            wrapper.AddReference(owner, id);

            return wrapper.promise;
        }
        else
        {
            string path = _route.LookupAssetBundlePath(assetBundleName);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            
            var promise = new Promise<AssetBundle>();
            promise.Resolve(assetBundle);

            wrapper = new AssetBundleWrapper()
            {
                assetBundle = assetBundle,
                loadingAssetSet = new Dictionary<string, Promise>(),
                path = assetBundleName,
                promise = promise,
            };
            wrapper.AddReference(owner, id);

            _assetBundleWrappers.Add(assetBundleName, wrapper);
            
            return promise;
        }
    }

    public IPromise<AssetBundle> GetAssetBundle(string assetBundleName, object owner)
    {
        Assert.IsNotNull(assetBundleName);
        Assert.IsNotNull(owner);

        var promise = new Promise<AssetBundle>();

        AssetBundle assetBundle = null;
        Exception ex = null;
        
        int id = GetNextIdAssetBundle();
        GetAssetBundle(id, assetBundleName, owner)
            .Then(theAssetBundle => { assetBundle = theAssetBundle; })
            .Catch(theEx => { ex = theEx; })
            .Finally(() =>
            {
                AssetBundleWrapper wrapper;
                if (_assetBundleWrappers.TryGetValue(assetBundleName, out wrapper))
                {
                    if (wrapper.IsContainTarget(owner, id))
                    {
                        promise.Resolve(assetBundle);
                    }
                    else
                    {
                        DestroyAssetBundleDependencies(assetBundleName, owner);
                        TryDestroyNoRefAssetBundle(assetBundleName, wrapper, true, owner);
                        promise.Reject(ex);
                    }
                }
                else
                {
                    DestroyAssetBundleDependencies(assetBundleName, owner);
                    promise.Reject(new Exception(
                        $"Load AssetBundle done but all refs destroyed assetBundleName: {assetBundleName}, owner: {owner}, id: {id}"));
                }
            });
        
        return promise;
    }

    public IPromise<T> GetAssetBundleAssetSync<T>(string assetPath, string assetName, object owner) where T : Object
    {
        Assert.IsNotNull(assetPath);
        Assert.IsNotNull(assetName);
        Assert.IsNotNull(owner);
        
        var promise = new Promise<T>();
        
        string assetBundleName = _route.LookupAssetBundleName(assetPath);

        GetAssetBundleSyncInternal(assetBundleName, owner)
            .Then(assetBundle =>
            {
                var wrapper = _assetBundleWrappers[assetBundleName];
                Assert.IsNotNull(wrapper);

                return LoaAssetSync<T>(assetName, assetBundle, wrapper);
            })
            .Then(asset =>
            {
                promise.Resolve(asset);
            })
            .Catch(ex => { promise.Reject(ex); });

        return promise;
    }

    public IPromise<T> GetAssetBundleAsset<T>(string assetPath, string assetName, object owner) where T : Object
    {
        Assert.IsNotNull(assetPath);
        Assert.IsNotNull(assetName);
        Assert.IsNotNull(owner);

        string assetBundleName = _route.LookupAssetBundleName(assetPath);
        
        var promise = new Promise<T>();

        T asset = null;
        Exception ex = null;

        int id = GetNextIdAssetBundle();
        GetAssetBundle(id, assetBundleName, owner)
            .Then(assetBundle =>
            {
                var wrapper = _assetBundleWrappers[assetBundleName];
                Assert.IsNotNull(wrapper);

                return LoaAssetAsync<T>(assetName, assetBundle, wrapper);
            })
            .Then(theAsset => { asset = theAsset; })
            .Catch(theEx => { ex = theEx; })
            .Finally(() => 
            {
                AssetBundleWrapper wrapper;
                if (_assetBundleWrappers.TryGetValue(assetBundleName, out wrapper))
                {
                    if (wrapper.IsContainTarget(owner, id))
                    {
                        promise.Resolve(asset);
                    }
                    else
                    {
                        DestroyAssetBundleDependencies(assetBundleName, owner);
                        TryDestroyNoRefAssetBundle(assetBundleName, wrapper, true, owner);

                        promise.Reject(ex);
                    }
                }
                else
                {
                    DestroyAssetBundleDependencies(assetBundleName, owner);
                    promise.Reject(new Exception(
                        $"Load Assets done but ALL refs destroyed assetBundleName: {assetBundleName}, assetName: {assetName}, owner: {owner}, id: {id}"));                    
                }
            });

        return promise;
    }
    
    private IPromise<T> LoaAssetSync<T>(string assetName, AssetBundle assetBundle, AssetBundleWrapper wrapper) where T : Object
    {
        WeakReference<Object> assetRef;
        if (wrapper.assetDict.TryGetValue(assetName, out assetRef))
        {
            Object assetObj;
            if (assetRef.TryGetTarget(out assetObj))
            {
                T asset = assetObj as T;

                if (asset)
                {
                    return Promise<T>.Resolved(asset);
                }
                else
                {
                    wrapper.assetDict.Remove(assetName);
                }
            }
            else
            {
                Debug.LogError($"Asset NOT release, Please check: {assetName}");
                wrapper.assetDict.Remove(assetName);
            }
        }
        
        Promise<T> loadPromise = new Promise<T>();
        
        T theAsset =  assetBundle.LoadAsset<T>(assetName);

        if(theAsset)
        {
            wrapper.assetDict[assetName] = new WeakReference<Object>(theAsset);
            
            loadPromise.Resolve(theAsset);
        }
        else
        {
            loadPromise.Reject(new Exception($"Fail to LoaAssetSync assetBundle: {assetBundle}, assetName: {assetName}"));
        }

        return loadPromise;
    }

    private IPromise<T> LoaAssetAsync<T>(string assetName, AssetBundle assetBundle, AssetBundleWrapper wrapper) where T : Object
    {
        T asset = RetrieveAsset<T>(wrapper, assetName);

        if (asset != null)
        {
            return Promise<T>.Resolved(asset);
        }
        else
        {
            Promise<T> assetPromise = new Promise<T>();
            
            Promise promise;
            if (wrapper.loadingAssetSet.TryGetValue(assetName, out promise))
            {
                DelayResolveAsset(wrapper, assetName, promise, assetPromise);
            }
            else
            {
                promise = new Promise();
                wrapper.loadingAssetSet.Add(assetName, promise);
                
                DelayResolveAsset(wrapper, assetName, promise, assetPromise);

                MainThreadDispatcher.StartUpdateMicroCoroutine(LoadAssetBundleAsset<T>(wrapper, promise, assetBundle, assetName));
            }

            return assetPromise;
        }
    }

    private void DelayResolveAsset<T>(AssetBundleWrapper wrapper, string assetName, Promise promise, Promise<T> assetPromise)
        where T : Object
    {
        promise
            .Then(() =>
            {
                T asset = RetrieveAsset<T>(wrapper, assetName);

                if (asset != null)
                {
                    assetPromise.Resolve(asset);
                }
                else
                {
                    assetPromise.Reject(new Exception($"asset == null, path: {wrapper.path} assetName: ${assetName}"));
                }
            })
            .Catch(ex => { assetPromise.Reject(ex); });
    }

    private T RetrieveAsset<T>(AssetBundleWrapper wrapper, string assetName) where T : Object
    {
        WeakReference<Object> assetRef;
        if (wrapper.assetDict.TryGetValue(assetName, out assetRef))
        {
            Object assetObj;
            if (assetRef.TryGetTarget(out assetObj))
            {
                T asset = assetObj as T;

                if (asset)
                {
                    return asset;
                }
                else
                {
                    wrapper.assetDict.Remove(assetName);
                }
            }
            else
            {
                Debug.LogError($"Asset NOT release, Please check: {assetName}");
                wrapper.assetDict.Remove(assetName);
            }
        }

        return null;
    }


    private IEnumerator LoadAssetBundleAsset<T>(AssetBundleWrapper wrapper, Promise promise, AssetBundle assetBundle, string assetName) where T : Object
    {
        AssetBundleRequest assetReq = assetBundle.LoadAssetAsync<T>(assetName);
        while (!assetReq.isDone)
            yield return null;

        wrapper.loadingAssetSet.Remove(assetName);

        // maybe destroyed
        T asset = assetReq.asset as T;

        if(asset)
        {
            // may be load twice
            wrapper.assetDict[assetName] = new WeakReference<Object>(asset);
            
            promise.Resolve();
        }
        else
        {
            promise.Reject(new Exception($"Fail to LoadAssetAsync assetBundle: {assetBundle}, assetName: {assetName}"));
        }
    }

    private IPromise<AssetBundle> GetAssetBundle(int id, string assetBundleName, object owner)
    {
        Assert.IsNotNull(assetBundleName);
        Assert.IsNotNull(owner);

        var promise = new Promise<AssetBundle>();

        GetAssetBundleInternal(assetBundleName, owner, id, true)
            .Then(_ =>
            {
                AssetBundleWrapper wrapper;
                if (_assetBundleWrappers.TryGetValue(assetBundleName, out wrapper))
                {
                    if (wrapper.IsContainTarget(owner, id))
                    {
                        promise.Resolve(wrapper.assetBundle);
                    }
                    else
                    {
                        promise.Reject(new Exception(
                            $"Load AssetBundle done but NO refs assetBundleName: {assetBundleName}, owner: {owner}, id: {id}"));
                    }
                }
                else
                {
                    promise.Reject(new Exception(
                        $"Load AssetBundle done but ALL refs destroyed assetBundleName: {assetBundleName}, owner: {owner}, id: {id}"));
                }
            })
            .Catch(ex =>
            {
                promise.Reject(ex);
            });

        return promise;
    }

    private void TryDestroyNoRefAssetBundle(string assetBundleName, AssetBundleWrapper wrapper, bool includeDep, object owner)
    {
        if (includeDep)
        {
            CheckNoRefWrapper(assetBundleName, wrapper, owner);

            var dependencies = _manifest.GetAllDependencies(assetBundleName);
            for (int i = 0, length = dependencies.Length; i < length; ++i)
            {
                string depBundle = dependencies[i];

                var depWrapper = _assetBundleWrappers[depBundle];
                Assert.IsNotNull(depWrapper);

                CheckNoRefWrapper(depBundle, depWrapper, owner);
            }
        }
        else
        {
            CheckNoRefWrapper(assetBundleName, wrapper, owner);
        }
    }

    private void CheckNoRefWrapper(string assetBundleName, AssetBundleWrapper wrapper, object owner)
    {
        if (wrapper.GetReferenceCount() == 0)
        {
            wrapper.Destroy();

            bool removed = _assetBundleWrappers.Remove(assetBundleName);
            Assert.IsTrue(removed);
            
            RaiseDecreaseEvent(assetBundleName, owner);
        }
        else
        {
            // still got reference
        }
    }

    private IPromise<AssetBundle> GetAssetBundleInternal(string assetBundleName, object owner, int id, bool includeDep)
    {
        AssetBundleWrapper wrapper;

        if(_assetBundleWrappers.TryGetValue(assetBundleName, out wrapper))
        {
            wrapper.AddReference(owner, id);

            if (includeDep)
            {
                var dependencies = _manifest.GetAllDependencies(assetBundleName);
                for (int i = 0, length = dependencies.Length; i < length; ++i)
                {
                    int depId = GetNextIdAssetBundle();
                    
                    string depBundleName = dependencies[i];
                    AssetBundleWrapper depWrapper;

                    if (_assetBundleWrappers.TryGetValue(depBundleName, out depWrapper))
                    {
                        depWrapper.AddReference(owner, depId);
                    }
                    else
                    {
                        // nothing
                    }
                }
            }
            else
            {
                // nothing
            }

            return wrapper.promise;
        }
        else
        {
            return BrandNewLoadAssetBundle(assetBundleName, owner, id, includeDep);
        }
    }

    private IPromise<AssetBundle> BrandNewLoadAssetBundle(string assetBundleName, object owner, int id, bool includeDep)
    {
        var promise = new Promise<AssetBundle>();

        AddAssetBundleWrapper(assetBundleName, owner, id, promise);

        if (includeDep)
        {
            string[] dependencies = _manifest.GetAllDependencies(assetBundleName);
            int length = dependencies.Length;

            if(length == 0)
            {
                MainThreadDispatcher.StartUpdateMicroCoroutine(BrandNewLoadAssetBundleSole(promise, assetBundleName));
            }
            else
            {
                IPromise<AssetBundle>[] promises = new IPromise<AssetBundle>[length + 1];

                // dependencies
                for (int i = 0; i < length; ++i)
                {
                    string depBundleName = dependencies[i];
                    int depId = GetNextIdAssetBundle();

                    promises[i] = GetAssetBundleInternal(depBundleName, owner, depId, false);
                }

                // self
                var selfPromise = new Promise<AssetBundle>();
                promises[length] = selfPromise;
                MainThreadDispatcher.StartUpdateMicroCoroutine(BrandNewLoadAssetBundleSole(selfPromise, assetBundleName));

                Exception e = null;
                AssetBundle assetBundle = null;

                Promise<AssetBundle>.All(promises)
                    .Then(assetBundles =>
                    {
                        assetBundle = GetIndexAssetBundle(assetBundles, length);
                    })
                    .Catch(ex =>
                    {
                        e = ex;
                    })
                    .Finally(() =>
                    {
                        if (assetBundle)
                        {
                            promise.Resolve(assetBundle);
                        }
                        else
                        {
                            promise.Reject(e);
                        }
                    });
            }
        }
        else
        {
            MainThreadDispatcher.StartUpdateMicroCoroutine(BrandNewLoadAssetBundleSole(promise, assetBundleName));
        }

        return promise;
    }

    private AssetBundle GetIndexAssetBundle(IEnumerable<AssetBundle> assetBundles, int index)
    {
        int i = 0;
        foreach (var assetBundle in assetBundles)
        {
            if (i == index)
            {
                return assetBundle;
            }
            ++i;
        }
        
        throw new Exception($"Unable to locate assetBundles {index}");
    }

    private void AddAssetBundleWrapper(string assetBundleName, object owner, int id, Promise<AssetBundle> promise)
    {
        var assetBundleWrapper = new AssetBundleWrapper()
        {
            path = assetBundleName,
            assetBundle = null,
            promise = promise,
            loadingAssetSet = new Dictionary<string, Promise>(),
        };
        assetBundleWrapper.AddReference(owner, id);
        _assetBundleWrappers.Add(assetBundleName, assetBundleWrapper);
    }

    private IEnumerator BrandNewLoadAssetBundleSole(Promise<AssetBundle> promise, string assetBundleName)
    {
        string path = _route.LookupAssetBundlePath(assetBundleName);

        var request = AssetBundle.LoadFromFileAsync(path);
        while (!request.isDone)
            yield return null;

        var assetBundle = request.assetBundle;

        var wrapper = _assetBundleWrappers[assetBundleName];
        Assert.IsNotNull(wrapper);

        wrapper.assetBundle = assetBundle;

        if (assetBundle)
        {
            promise.Resolve(assetBundle);
        }
        else
        {
            promise.Reject(new Exception(
                $"Failed to load AssetBundle, assetBundleName: {assetBundleName}, path: {path}"));
        }
    }

    public void DestroyAssetBundle(string assetPath, object owner)
    {
        Assert.IsNotNull(assetPath);
        Assert.IsNotNull(owner);

        string assetBundleName = _route.LookupAssetBundleName(assetPath);

        bool selfDestroy = DestroyAssetBundleSolo(assetBundleName, owner);

        if (selfDestroy)
        {
            DestroyAssetBundleDependencies(assetBundleName, owner);
        }
        else
        {
            // waiting
        }
    }

    private void DestroyAssetBundleDependencies(string assetBundleName, object owner)
    {
        string[] dependencies = _manifest.GetAllDependencies(assetBundleName);
        for (int i = 0, length = dependencies.Length; i < length; ++i)
        {
            string depAssetBundle = dependencies[i];
            DestroyAssetBundleSolo(depAssetBundle, owner);
        }
    }

    private bool DestroyAssetBundleSolo(string assetBundleName, object owner)
    {
        AssetBundleWrapper wrapper;
        if(_assetBundleWrappers.TryGetValue(assetBundleName, out wrapper))
        {
            bool finish = (wrapper.promise.CurState == PromiseState.Rejected || wrapper.promise.CurState == PromiseState.Resolved);

            if (finish && !wrapper.IsLoadingAsset())
            {
                wrapper.RemoveReference(owner);

                TryDestroyNoRefAssetBundle(assetBundleName, wrapper, false, owner);

                return true;
            }
            else
            {
                // still loading
                wrapper.RemoveReference(owner);

                return false;
            }
        }
        else
        {
            // no ref
            Debug.LogError($"Destroy Not Loaded AssetBundle assetBundleName: {assetBundleName} owner: {owner}");
            return true;
        }
    }

    private static void RaiseIncreaseEvent(string assetBundleName, object owner)
    {
        MainThreadDispatcher.StartUpdateMicroCoroutine(RaiseIncreaseEventInternal(assetBundleName, owner));
    }

    private static IEnumerator RaiseIncreaseEventInternal(string assetBundleName, object owner)
    {
        yield return null;
        
        RefIncreaseEvent.assetBundleName = assetBundleName;
        RefIncreaseEvent.owner = owner;
        
        Events.instance.Raise(RefIncreaseEvent);
    }

    private static void RaiseDecreaseEvent(string assetBundleName, object owner)
    {
        MainThreadDispatcher.StartUpdateMicroCoroutine(RaiseDecreaseEventInternal(assetBundleName, owner));
    }

    private static IEnumerator RaiseDecreaseEventInternal(string assetBundleName, object owner)
    {
        yield return null;
        
        RefDecreaseEvent.assetBundleName = assetBundleName;
        RefDecreaseEvent.owner = owner;
        
        Events.instance.Raise(RefDecreaseEvent);
    }

    private static int GetNextIdAssetBundle()
    {
        assetBundleId++;

        return assetBundleId;
    }

    public Dictionary<string, List<System.Tuple<object, int>>> GetReferenceDict()
    {
        var dict = new Dictionary<string, List<System.Tuple<object, int>>>();
        
        foreach (var item in _assetBundleWrappers)
        {
            string assetBundleName = item.Key;
            AssetBundleWrapper wrapper = item.Value;

            var list = new List<System.Tuple<object, int>>();

            foreach (AssetBundleWrapper.RefData refData in wrapper.GetReferenceList())
            {
                object owner = refData.owner;
                int internalCount = refData.internalRefList.Count;

                for (int i = 0; i < internalCount; ++i)
                {
                    int internalId = refData.internalRefList[i];
                    var data = System.Tuple.Create(owner, internalId);
                    
                    list.Add(data);
                }
            }
            
            dict.Add(assetBundleName, list);
        }

        return dict;
    }
}
