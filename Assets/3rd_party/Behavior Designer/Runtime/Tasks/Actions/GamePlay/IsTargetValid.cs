using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class IsTargetValid : Action
    {
        public SharedGameObject target;

        public override TaskStatus OnUpdate()
        {
            if (target != null && target.Value != null)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
    }
}
