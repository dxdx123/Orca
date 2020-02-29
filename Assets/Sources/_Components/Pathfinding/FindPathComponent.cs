using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

[Game]
public class FindPathComponent : IComponent
{
    public float x;
    public float y;

    public bool exactDest;
}
