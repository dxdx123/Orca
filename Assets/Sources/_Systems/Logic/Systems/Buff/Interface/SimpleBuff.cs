using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBuffOccur : IBuffOccur
{
    public void OnOccur(GameEntity gameEntity)
    {
        Debug.Log($"!!! OnOccur");
    }
}
