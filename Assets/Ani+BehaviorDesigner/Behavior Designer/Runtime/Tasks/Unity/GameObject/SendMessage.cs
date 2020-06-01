using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("SendMessage to the GameObject. Returns Success.")]
    public class SendMessage: Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("Method")]
        public SharedString method;

        public override void OnStart()
        {

        }
        public override TaskStatus OnUpdate()
        {
            GetDefaultGameObject(targetGameObject.Value).SendMessage(method.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            method = null;
        }
    }
}