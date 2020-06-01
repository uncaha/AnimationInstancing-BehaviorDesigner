using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AniPlayable.InstanceAnimation
{
    public class AttachmentFllow : MonoBehaviour
    {
        public GameObject attachment = null;

        private UpdateObject updateObject;
        private bool initialize = false;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (initialize) return;
            Initialize();
            initialize = true;
        }
        private void Initialize()
        {

            AnimationInstancing instance = GetComponent<AnimationInstancing>();
            if (instance)
            {
                int count = instance.GetAnimationCount();
                //instance.PlayAnimation(Random.Range(0, count));
                AnimationInstancing attachmentScript = attachment.GetComponent<AnimationInstancing>();
                instance.Attach("ik_hand_r", attachmentScript);
            }

            updateObject = new UpdateObject(this, UpdateRender);
            PlayableUpdateManager.Reg(updateObject);
        }

        private void OnDestroy()
        {
            PlayableUpdateManager.UnReg(updateObject);
        }
        public void UpdateRender(float dt)
        {

        }
    }
}