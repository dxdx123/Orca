using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSystems : Feature
{
    public RenderSystems(Contexts contexts)
        : base("Render Systems")
    {
        Add(new AnimateViewSystem(contexts));
        Add(new CharacterViewSystem(contexts));
        Add(new DirectionViewSystem(contexts));
        Add(new SortViewSystem(contexts));
    }
}
