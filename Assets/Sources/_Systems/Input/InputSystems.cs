using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystems : Feature
{
    public InputSystems(Contexts contexts)
        : base("Input Systems")
    {
        Add(new NetworkInputSystem(contexts));
        
        Add(new EmitInputSystem(contexts));
        Add(new CleanInputSystem(contexts));
        // Add(new TouchScreenSystem(contexts));
        
        // Add(new TouchToDieSystem(contexts));
        // Add(new TouchEnemySystem(contexts));
        
        Add(new TranslateInputMoveSystem(contexts));
        Add(new TranslateInputActionSystem(contexts));
    }
}
