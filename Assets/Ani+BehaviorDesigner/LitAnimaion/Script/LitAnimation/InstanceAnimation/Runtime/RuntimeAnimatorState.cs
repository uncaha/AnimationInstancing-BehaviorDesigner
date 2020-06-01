
using System;
using AniPlayable.Module;
using System.Collections.Generic;
namespace AniPlayable.InstanceAnimation
{
    public class RuntimeAnimatorState : Node
    {
        public readonly int layerIndex;
        public readonly int machineIndex;
        public readonly string stateName;
        public readonly int nameHash;
        public readonly int motionHash;
        public int motionIndex;
        private AnimationStateInfo stateInfo;
        private AnimatorTransition[] animatorTransitions;
        private int transLength = 0;

        public RuntimeAnimatorState(AnimationStateInfo pInfo)
        {
            if (pInfo == null) return;
            stateInfo = pInfo;
            layerIndex = stateInfo.layerIndex;
            machineIndex = stateInfo.machineIndex;
            stateName = stateInfo.name;
            nameHash = stateInfo.hashName;

            transLength = stateInfo.transtionList.Count;

            motionHash = stateInfo.motionName.GetHashCode();
        }
        public override void InitNode(AnimationInstancing pAnimator)
        {
            motionIndex = pAnimator.FindAnimationInfo(motionHash);
            animatorTransitions = new AnimatorTransition[transLength];
            for (int i = 0; i < transLength; i++)
            {
                var item = stateInfo.transtionList[i];
                AnimatorTransition item2 = new AnimatorTransition(parameters, item, i);
                animatorTransitions[i] = item2;
            }
        }

        public AnimatorTransition CheckTransition(float p)
        {
            if (transLength == 0) return null;

            for (int i = 0; i < transLength; i++)
            {
                var ttrans = animatorTransitions[i];
                if (ttrans.CheckCondition())
                {
                    if (ttrans.exitTime < p)
                    {
                        return ttrans;
                    }
                }

            }
            return null;
        }
    }
}