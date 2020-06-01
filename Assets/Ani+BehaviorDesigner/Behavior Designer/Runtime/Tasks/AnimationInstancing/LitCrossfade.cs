using UnityEngine;
using AniPlayable.InstanceAnimation;
namespace BehaviorDesigner.Runtime.Tasks.AniInstancing
{
    [TaskCategory("AniPlable/Instancing")]
    [TaskDescription("Creates a dynamic transition between the current state and the destination state. Returns Success.")]
    public class Crossfade : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the state")]
        public SharedString stateName;

        public WrapMode wrapMode = WrapMode.Once;

        [Tooltip("The duration of the transition. Value is in source state normalized time")]
        public SharedFloat transitionDuration;
        [Tooltip("The layer where the state is")]
        public int layer = -1;
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

            animator.CrossFade(stateName.Value, transitionDuration.Value, layer == -1 ? 0 : layer);
            animator.Mode = wrapMode;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            stateName = "";
            transitionDuration = 0;
            layer = -1;
        }
    }
}