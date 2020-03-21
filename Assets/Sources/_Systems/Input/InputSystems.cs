using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystems : Feature
{
    public InputSystems(Contexts contexts)
        : base("Input Systems")
    {
        Add(new EmitInputSystem(contexts));
        Add(new TouchScreenSystem(contexts));
        
        Add(new TranslateInputMoveSystem(contexts));
        Add(new TranslateInputActionSystem(contexts));
    }
}
