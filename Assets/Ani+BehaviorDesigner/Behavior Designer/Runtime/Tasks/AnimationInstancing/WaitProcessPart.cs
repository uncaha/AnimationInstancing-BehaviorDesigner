using UnityEngine;
using AniPlayable.InstanceAnimation;
namespace BehaviorDesigner.Runtime.Tasks.AniInstancing
{
    [TaskCategory("AniPlable/Instancing")]
    [TaskIcon("{SkinColor}WaitIcon.png")]
    public class WaitProcessPart: Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        [Tooltip("The process.")]
        public SharedFloat processPart;
        private AnimationInstancing animator;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject)
            {
                animator = currentGameObject.GetComponentInChildren<AnimationInstancing>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (animator == null)
            {
                Debug.LogWarning("AnimationInstancing is null");
                return TaskStatus.Failure;
            }

            if(animator.curProcess < processPart.Value) return TaskStatus.Running;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            processPart = null;
        }
    }
}