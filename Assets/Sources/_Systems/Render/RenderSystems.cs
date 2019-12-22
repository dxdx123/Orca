using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSystems : Feature
{
    public RenderSystems(Contexts contexts)
        : base("Render Systems")
    {
        Add(new CharacterViewSystem(contexts));
    }
}
