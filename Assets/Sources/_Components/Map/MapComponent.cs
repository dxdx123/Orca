using System.Collections;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

[Game, Unique]
public class MapComponent : IComponent
{
    public string mapName;
    public float width;
    public float height;
}
