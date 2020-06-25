using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

[Game]
public class ViewEventTriggerComponent : IComponent
{
    public string eventName;

    public string eventInfo;
    public float eventFloat;
    public int eventInt;
}
