using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class IsFarAwayTarget : Action
    {
        public SharedGameObject target;
        public SharedFloat distance;

        public override TaskStatus OnUpdate()
        {
            Vector3 srcPos = transform.position;
            Vector3 destPos = target.Value.transform.position;

            float diff = (destPos - srcPos).sqrMagnitude;

            float sqrDistance = distance.Value * distance.Value;

            if (diff > sqrDistance)
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
