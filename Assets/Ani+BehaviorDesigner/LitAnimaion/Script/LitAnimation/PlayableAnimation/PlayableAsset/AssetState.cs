using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AniPlayable.Module;
namespace AniPlayable
{
    public class AssetState : ScriptableObject
    {
        [System.Serializable]
        public class AnimationState
        {
            public AnimationClip clip;
            public float threshold;
            public float speed;
            public AssetBlendTree.BlendTreeType blendTreeType;
            public string stateName;
            public AssetTransitions.Transtions[] transtions;

            public void AddState(PlayableAnimator playableAnimator, string groupName, int layer)
            {
                string tstateName = string.IsNullOrEmpty(stateName) ? clip.name : stateName;
                PlayableStateController.StateInfo tinfo = playableAnimator.AddState(clip, tstateName, groupName, layer);
                tinfo.speed = speed;
                if(transtions != null && transtions.Length > 0)
                {
                    AnimatorTransition[] ttransObj = new AnimatorTransition[transtions.Length];
                    tinfo.transtions = ttransObj;
                    for (int i = 0; i < ttransObj.Length; i++)
                    {
                        ttransObj[i] = new AnimatorTransition(playableAnimator.StateController.Params,transtions[i],i);
                    }
                }
            }
        }

        public AnimationState data;
    }
}