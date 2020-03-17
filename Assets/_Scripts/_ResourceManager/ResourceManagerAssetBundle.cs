using RSG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Runtime.InteropServices;
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

    private static int _idAssetBundle = 0;
    public const int DefaultListSize = 4;

    class AssetBundleWrapper
    {
        class RefData
        {
            public object owner;
            public List<int> internalRefList = new List<int>(DefaultListSize);
        }

        public string path;
        public AssetBundle assetBundle;

        public Promise<AssetBundle> promise;
        public HashSet<string> loadingAssetSet;

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
        }

        public int GetReferenceCount()
        {
            return _referenceList.Count;
        }

        public int GetTotalReferenceCount()
        {
            int sum = 0;
            for (int i = 0, length = _referenceList.Count; i < length; ++i)
            {
                sum += _referenceList[i].internalRefList.Count;
            }

            return sum;
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

    private Dictionary<string, AssetBundleWrapper> _assetBundleWrappers = new Dictionary<string, AssetBundleWrapper>(DefaultListSize);

    private ResourceManagerAssetBundle() { }

    public void Initialize(AssetBundleManifest manifest, IAssetBundleRoute route)
    {
        Assert.IsNotNull(manifest);
        Assert.IsNotNull(route);

        _manifest = new AssetBundleManifestCache(manifest);
        _route = route;
    }

    public IPromise<AssetBundle> GetAssetBundleSync(string assetPath, object owner)
    {
        Assert.IsNotNull(assetPath);
        Assert.IsNotNull(owner);

        string assetBundleName = _route.LookupAssetBundleName(assetPath);

        int id = GetNextIdAssetBundle();
        
        var dependencies = _manifest.GetAllDependencies(assetBundleName);
        int length = dependencies.Length;

        if (length == 0)
        {
            return LoadAssetBundleSyncInternal(assetBundleName, owner, id);
        }
        else
        {
            IPromise<AssetBundle>[] promises = new IPromise<AssetBundle>[length + 1];

            // dependency
            for (int i = 0; i < length; ++i)
            {
                string depBundleName = dependencies[i];
                promises[i] = LoadAssetBundleSyncInternal(depBundleName, owner, id);
            }
            
            // self
            promises[length] = LoadAssetBundleSyncInternal(assetBundleName, owner, id);

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

    private IPromise<AssetBundle> LoadAssetBundleSyncInternal(string assetBundleName, object owner, int id)
    {
        AssetBundleWrapper wrapper;
        if (_assetBundleWrappers.TryGetValue(assetBundleName, out wrapper))
        {
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
                loadingAssetSet = new HashSet<string>(),
                path = assetBundleName,
                promise = promise,
            };
            wrapper.AddReference(owner, id);

            _assetBundleWrappers.Add(assetBundleName, wrapper);

            return promise;
        }
    }

    public IPromise<AssetBundle> GetAssetBundle(string assetPath, object owner)
    {
        Assert.IsNotNull(assetPath);
        Assert.IsNotNull(owner);

        string assetBundleName = _route.LookupAssetBundleName(assetPath);

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
                    TryDestroyNoRefAssetBundle(assetBundleName, wrapper, true);

                    if (wrapper.IsContainTarget(owner, id))
                    {
                        promise.Resolve(assetBundle);
                    }
                    else
                    {
                        promise.Reject(ex);
                    }
                }
                else
                {
                    promise.Reject(new Exception(
                        $"Load AssetBundle done but all refs destroyed assetBundleName: {assetBundleName}, owner: {owner}, id: {id}"));
                }
            });
        
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
                    TryDestroyNoRefAssetBundle(assetBundleName, wrapper, true);

                    if (wrapper.IsContainTarget(owner, id))
                    {
                        promise.Resolve(asset);
                    }
                    else
                    {
                        promise.Reject(ex);
                    }
                }
                else
                {
                    promise.Reject(new Exception(
                        $"Load Assets done but ALL refs destroyed assetBundleName: {assetBundleName}, assetName: {assetName}, owner: {owner}, id: {id}"));                    
                }
            });

        return promise;
    }

    private IPromise<T> LoaAssetAsync<T>(string assetName, AssetBundle assetBundle, AssetBundleWrapper wrapper) where T : Object
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
                    return Promise<T>.Rejected(
                        new Exception($"LoadAssetSync can't cast assetObj {assetObj.GetType()} to {typeof(T)}"));
                }
            }
            else
            {
                Debug.LogError($"Asset NOT release, Please check: {assetName}");
                wrapper.assetDict.Remove(assetName);
            }
        }
        
        wrapper.loadingAssetSet.Add(assetName);

        Promise<T> loadPromise = new Promise<T>();
        MainThreadDispatcher.StartUpdateMicroCoroutine(LoadAssetBundleAsset(wrapper, loadPromise, assetBundle, assetName));

        return loadPromise;
    }

    private IEnumerator LoadAssetBundleAsset<T>(AssetBundleWrapper wrapper, Promise<T> promise, AssetBundle assetBundle, string assetName) where T : Object
    {
        var assetReq = assetBundle.LoadAssetAsync<T>(assetName);
        while (!assetReq.isDone)
            yield return null;

        wrapper.loadingAssetSet.Remove(assetName);

        // maybe destroyed
        T asset = assetReq.asset as T;

        if(asset)
        {
            // may be load twice
            wrapper.assetDict[assetName] = new WeakReference<Object>(asset);
            
            promise.Resolve(asset);
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
            .Then(assetBundle => { })
            .Catch(Debug.LogException)
            .Finally(() => 
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
                        TryDestroyNoRefAssetBundle(assetBundleName, wrapper, true);
                        promise.Reject(new Exception(
                            $"Load AssetBundle done but NO refs assetBundleName: {assetBundleName}, owner: {owner}, id: {id}"));                    
                    }
                }
                else
                {
                    promise.Reject(new Exception(
                        $"Load AssetBundle done but ALL refs destroyed assetBundleName: {assetBundleName}, owner: {owner}, id: {id}"));                    
                }
            });

        return promise;
    }

    private void TryDestroyNoRefAssetBundle(string assetBundleName, AssetBundleWrapper wrapper, bool includeDep)
    {
        if (includeDep)
        {
            CheckNoRefWrapper(assetBundleName, wrapper);

            var dependencies = _manifest.GetAllDependencies(assetBundleName);
            for (int i = 0, length = dependencies.Length; i < length; ++i)
            {
                string depBundle = dependencies[i];

                var depWrapper = _assetBundleWrappers[depBundle];
                Assert.IsNotNull(depWrapper);

                CheckNoRefWrapper(depBundle, depWrapper);
            }
        }
        else
        {
            CheckNoRefWrapper(assetBundleName, wrapper);
        }
    }

    private void CheckNoRefWrapper(string assetBundleName, AssetBundleWrapper wrapper)
    {
        if (wrapper.GetReferenceCount() == 0)
        {
            wrapper.Destroy();

            bool remove = _assetBundleWrappers.Remove(assetBundleName);
            Assert.IsTrue(remove);
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

            var dependencies = _manifest.GetAllDependencies(assetBundleName);
            for(int i=0, length=dependencies.Length; i<length; ++i)
            {
                string depBundles = dependencies[i];
                var depWrapper = _assetBundleWrappers[depBundles];
                Assert.IsNotNull(depWrapper);

                depWrapper.AddReference(owner, id);
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
                        Debug.LogException(ex);
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
        AssetBundle assetBundle = null;

        int i = 0;
        var enumerator = assetBundles.GetEnumerator();
        while(enumerator.MoveNext())
        {
            if(i == index)
            {
                assetBundle = enumerator.Current;
                break;
            }
            ++i;
        }
        enumerator.Dispose();

        Assert.AreEqual(index, i);
        return assetBundle;
    }

    private void AddAssetBundleWrapper(string assetBundleName, object owner, int id, Promise<AssetBundle> promise)
    {
        var assetBundleWrapper = new AssetBundleWrapper()
        {
            path = assetBundleName,
            assetBundle = null,
            promise = promise,
            loadingAssetSet = new HashSet<string>(),
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

        string[] dependencies = _manifest.GetAllDependencies(assetBundleName);
        for(int i=0, length = dependencies.Length; i<length; ++i)
        {
            string depAssetBundle = dependencies[i];
            DestroyAssetBundleSolo(depAssetBundle, owner);
        }

        DestroyAssetBundleSolo(assetBundleName, owner);
    }

    private void DestroyAssetBundleSolo(string assetBundleName, object owner)
    {
        AssetBundleWrapper wrapper;
        if(_assetBundleWrappers.TryGetValue(assetBundleName, out wrapper))
        {
            bool finish = (wrapper.promise.CurState == PromiseState.Rejected || wrapper.promise.CurState == PromiseState.Resolved);

            if (finish && !wrapper.IsLoadingAsset())
            {
                wrapper.RemoveReference(owner);

                TryDestroyNoRefAssetBundle(assetBundleName, wrapper, false);
            }
            else
            {
                // still loading
                wrapper.RemoveReference(owner);
            }
        }
        else
        {
            // no ref
            Debug.LogError($"Destroy Not Loaded AssetBundle assetBundleName: {assetBundleName} owner: {owner}");
        }
    }

    private static int GetNextIdAssetBundle()
    {
        _idAssetBundle++;

        return _idAssetBundle;
    }

    public int GetAssetBundleCount()
    {
        int sum = 0;

        foreach (var item in _assetBundleWrappers)
        {
            var wrapper = item.Value;
            sum += wrapper.GetTotalReferenceCount();
        }

        return sum;
    }
}
