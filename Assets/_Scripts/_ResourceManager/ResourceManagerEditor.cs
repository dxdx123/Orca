using System;
using System.Collections;
using System.Collections.Generic;
using RSG;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

#if UNITY_EDITOR

using UnityEditor;

public class ResourceManagerEditor
{
    private static ResourceManagerEditor _instance;

    public static ResourceManagerEditor Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourceManagerEditor();
            }

            return _instance;
        }
    }
    
    private Dictionary<string, AssetWrapper> _assetWrappers = new Dictionary<string, AssetWrapper>(DefaultListSize);

    private ResourceManagerEditor()
    {}

    private static int _idAsset = 0;
    
    public const int DefaultListSize = 4;
    
    public const int ASSET_LOAD_FRAME = 2;

    class AssetWrapper
    {
        class RefData
        {
            public object owner;
            public List<int> internalRefList = new List<int>(DefaultListSize);
        }

        public string path;

        public Object asset;
        public Promise<Object> promise;
        
        private List<RefData> _referenceList = new List<RefData>(DefaultListSize);
        
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

        public void Destroy()
        {
            bool loadDone = (promise.CurState == PromiseState.Rejected || promise.CurState == PromiseState.Resolved);
            Assert.AreEqual(true, loadDone);

            Assert.IsNotNull(asset);
            Assert.AreEqual(0, _referenceList.Count);
        }
        
        public void RemoveReference(object owner)
        {
            RefData refData = FindRefData(owner);
            if (refData == null)
            {
                Debug.LogError($"Destroy NO refs Asset: {path}, owner: {owner}");                
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
    
    public IPromise<T> GetAssetEditor<T>(string assetPath, object owner) where T : Object
    {
        Assert.IsNotNull(assetPath);
        Assert.IsNotNull(owner);

        var promise = new Promise<T>();

        var id = GetNextIdAsset();

        GetAssetInternal<T>(assetPath, owner, id)
            .Then(asset => { })
            .Catch(Debug.LogException)
            .Finally(() =>
            {
                AssetWrapper wrapper;

                if (_assetWrappers.TryGetValue(assetPath, out wrapper))
                {
                    if (wrapper.IsContainTarget(owner, id))
                    {
                        promise.Resolve(wrapper.asset as T);
                    }
                    else
                    {
                        CheckNoRefWrapper(assetPath, wrapper);
                        
                        promise.Reject(new Exception(
                            $"Load asset done but NO refs assetPath: {assetPath}, owner: {owner}, id: {id}")); 
                    }
                }
                else
                {
                    promise.Reject(new Exception(
                    $"Load asset done but ALL refs destroyed assetPath: {assetPath}, owner: {owner}, id: {id}"));                    
                }
            });
        
        return promise;
    }

    private IPromise<Object> GetAssetInternal<T>(string assetPath, object owner, int id) where T : Object
    {
        AssetWrapper wrapper;

        if (_assetWrappers.TryGetValue(assetPath, out wrapper))
        {
            wrapper.AddReference(owner, id);

            return wrapper.promise;
        }
        else
        {
            return BrandNewLoadAsset<T>(assetPath, owner, id);
        }
    }

    private IPromise<Object> BrandNewLoadAsset<T>(string assetPath, object owner, int id) where T : Object
    {
        var promise = new Promise<Object>();

        AddAssetBundleWrapper(assetPath, owner, id, promise);
        
        MainThreadDispatcher.StartUpdateMicroCoroutine(BrandNewLoadAssetSole<T>(promise, assetPath));
        
        return promise;
    }

    private IEnumerator BrandNewLoadAssetSole<T>(Promise<Object> promise, string assetPath) where T : Object
    {
        for(int i=0; i<ASSET_LOAD_FRAME; ++i)
            yield return null;

        Object asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

        AssetWrapper wrapper = _assetWrappers[assetPath];
        Assert.IsNotNull(wrapper);

        wrapper.asset = asset;

        if (asset)
        {
            promise.Resolve(asset);
        }
        else
        {
            promise.Reject(new Exception($"Fail to load asset: {assetPath} type: {typeof(T)}"));
        }
    }

    private void AddAssetBundleWrapper(string assetPath, object owner, int id, Promise<Object> promise)
    {
        var assetWrapper = new AssetWrapper()
        {
            path = assetPath,
            asset = null,
            promise = promise,
        };
        assetWrapper.AddReference(owner, id);
        _assetWrappers.Add(assetPath, assetWrapper);
    }

    public void DestroyAssetEditor(string assetPath, object owner)
    {
        Assert.IsNotNull(assetPath);
        Assert.IsNotNull(owner);
        
        DestroyAssetBundleSolo(assetPath, owner);
    }
    
    private void DestroyAssetBundleSolo(string assetPath, object owner)
    {
        AssetWrapper wrapper;
        if(_assetWrappers.TryGetValue(assetPath, out wrapper))
        {
            bool finish = wrapper.promise.CurState == PromiseState.Rejected ||
                          wrapper.promise.CurState == PromiseState.Resolved;

            if (finish)
            {
                wrapper.RemoveReference(owner);
                
                CheckNoRefWrapper(assetPath, wrapper);
            }
            else
            {
                wrapper.RemoveReference(owner);
            }
        }
        else
        {
            // no ref
            Debug.LogError($"Destroy Not Loaded Asset assetPath: {assetPath} owner: {owner}");
        }
    }
    
    private void CheckNoRefWrapper(string assetPath, AssetWrapper wrapper)
    {
        if (wrapper.GetReferenceCount() == 0)
        {
            wrapper.Destroy();

            bool remove = _assetWrappers.Remove(assetPath);
            Assert.IsTrue(remove);
        }
        else
        {
            // still got reference
        }
    }
    
    private static int GetNextIdAsset()
    {
        _idAsset++;

        return _idAsset;
    }
}

#endif
