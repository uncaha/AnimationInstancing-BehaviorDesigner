
using UnityEngine;
using AniPlayable.InstanceAnimation;
namespace BehaviorDesigner.Runtime.Tasks.AniInstancing
{
    [TaskCategory("AniPlable/Instancing")]
    [TaskDescription("Stores the float parameter on an animator. Returns Success.")]
    public class CompareInt : Action
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
        public int storeValue;

        [Tooltip("GetValue Greater then storeValue,or less.")]
        public AnimatorConditionMode mode = AnimatorConditionMode.Equals;
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

            int tvalue = animator.GetInt(paramaterName.Value);

            switch (mode)
            {
                case AnimatorConditionMode.Equals:
                    if (storeValue == tvalue)
                        return TaskStatus.Success;
                    break;
                case AnimatorConditionMode.NotEqual:
                    if (storeValue != tvalue)
                        return TaskStatus.Success;
                    break;
                case AnimatorConditionMode.Greater:
                    if (tvalue > storeValue)
                        return TaskStatus.Success;
                    break;
                case AnimatorConditionMode.Less:
                    if (tvalue < storeValue)
                        return TaskStatus.Success;
                    break;
                default:
                    return TaskStatus.Success;
            }
            

            return TaskStatus.Running;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            paramaterName = "";
            storeValue = 0;
        }
    }
}