using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

[Game]
public class MapComponent : IComponent
{
    public string mapName;
    public float width;
    public float height;
}
