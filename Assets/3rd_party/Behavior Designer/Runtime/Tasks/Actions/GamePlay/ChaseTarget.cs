using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class ChaseTarget : Action
    {
        public const float Range = 0.2f;
        
        public SharedGameObject target;
        
        public override TaskStatus OnUpdate()
        {
            var destPos = target.Value.transform.position;
            
            var entity = gameObject.GetEntityLink().entity as GameEntity;
            Assert.IsNotNull(entity);

            var srcPos = new Vector3(entity.position.x, entity.position.y, 0.0f);

            if ((destPos - srcPos).sqrMagnitude <= Range)
            {
                GameEntity targetEntity = target.Value.gameObject.GetEntityLink().entity as GameEntity;
                Assert.IsNotNull(targetEntity);

                if (targetEntity.state.state == CharacterState.Run)
                {
                    entity.ReplaceFindPath(destPos.x, destPos.y, false);
                    return TaskStatus.Running;
                }
                else
                {
                    return TaskStatus.Success;
                }
            }
            else
            {
                entity.ReplaceFindPath(destPos.x, destPos.y, false);
                return TaskStatus.Running;
            }
        }
    }
}
