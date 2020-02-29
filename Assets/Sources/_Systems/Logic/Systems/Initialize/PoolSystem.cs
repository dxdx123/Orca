using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class PoolSystem : IInitializeSystem
{
    public PoolSystem(Contexts contexts)
    {
        
    }
    
    public void Initialize()
    {
        AssetPoolManager.Instance.Initialize(10, 10, 100)
            .Then(() =>
            {
                Debug.Log("!!! initialize done");
            })
            .Catch(ex => {Debug.LogException(ex);});
    }
}
