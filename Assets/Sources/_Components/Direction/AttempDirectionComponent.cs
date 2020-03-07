using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

[Game]
public class AttempDirectionComponent : IComponent
{
    public Vector2 srcPosition;
    public Vector2 destPosition;
}
