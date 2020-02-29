using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class EmptySuccess : Action
    {
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
