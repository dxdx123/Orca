using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

[Game]
public class PathfindingMoveComponent : IComponent
{
    public float deltaTime;
    public Vector3 nextPosition;
}
