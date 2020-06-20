using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using UnityEngine.Assertions;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class WaitUntilCharacterState : Action
    {
        public SharedGameObject target;
        public SharedString state;

        public override TaskStatus OnUpdate()
        {
            var srcEntity = gameObject.GetEntityLink().entity as GameEntity;
            Assert.IsNotNull(srcEntity);
            
            Assert.IsTrue(srcEntity.hasState);

            CharacterState characterState = srcEntity.state.state;

            if (characterState.GetCacheString() == state.Value)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Running;
            }
        }
    }
}
