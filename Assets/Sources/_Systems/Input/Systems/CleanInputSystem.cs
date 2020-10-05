using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CleanInputSystem : ICleanupSystem
{
    private readonly IGroup<InputEntity> _inputGroup;
    private readonly List<InputEntity> _cleanBuffer = new List<InputEntity>();
    
    public CleanInputSystem(Contexts contexts)
    {
        _inputGroup = contexts.input.GetGroup(InputMatcher.AnyOf(InputMatcher.InputMove, InputMatcher.InputAction));
    }
    
    public void Cleanup()
    {
        foreach (var e in _inputGroup.GetEntities(_cleanBuffer))
        {
            e.Destroy();
        }
    }
}
