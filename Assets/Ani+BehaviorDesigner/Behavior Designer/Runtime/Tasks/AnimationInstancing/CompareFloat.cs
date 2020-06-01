using UnityEngine;
using AniPlayable.InstanceAnimation;
namespace BehaviorDesigner.Runtime.Tasks.AniInstancing
{
    [TaskCategory("AniPlable/Instancing")]
    [TaskDescription("Stores the float parameter on an animator. Returns Success.")]
    public class CompareFloat : Action
    {
        public enum AnimatorConditionMode
        {
            If = 1,
            IfNot = 2,
            Greater = 3,
            Less = 4,
            Equals = 6,
            NotEqual = 7
        }
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the parameter")]
        public SharedString paramaterName;
        [Tooltip("The value of the float parameter")]
        public float storeValue;

        [Tooltip("GetValue Greater then storeValue,or less.")]
        public AnimatorConditionMode mode = AnimatorConditionMode.Greater;
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
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

            float tvalue = animator.GetFloat(paramaterName.Value);

            if(mode == AnimatorConditionMode.Greater)
            {
                if(tvalue < storeValue)
                {
                    return TaskStatus.Running;
                }
            }
            else
            {
                if(tvalue > storeValue)
                {
                    return TaskStatus.Running;
                }
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            paramaterName = "";
            storeValue = 0;
        }
    }
}