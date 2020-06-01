using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AniPlayable
{
    [CreateAssetMenu(fileName = "StateGroup", menuName = "PlayableAnimator/StateGroup", order = 2)]
    public class AssetStateGroup : ScriptableObject
    {
        public string groupName;
        public AssetState.AnimationState[] motions;
        public AssetBlendTree[] blendTrees;

        public void AddStates(PlayableAnimator playableAnimator, int layer)
        {
            if (blendTrees != null)
            {
                for (int i = 0; i < blendTrees.Length; i++)
                {
                    blendTrees[i].AddStates(playableAnimator, layer, groupName);
                }
            }

            if (motions != null)
            {
                for (int i = 0; i < motions.Length; i++)
                {
                    motions[i].AddState(playableAnimator,groupName,layer);
                }
            }
        }
    }
}

