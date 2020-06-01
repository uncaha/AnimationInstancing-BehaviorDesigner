using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AniPlayable
{
    [CreateAssetMenu(fileName = "StateController", menuName = "PlayableAnimator/StateController", order = 4)]
    public class AssetStateController : ScriptableObject
    {
        public AssetParameter parameters;
        public AssetStateLayer[] stateLayers;

        void AddParameters(PlayableAnimator playableAnimator)
        {
            if (parameters != null && parameters.parameters != null)
            {
                var tlist = parameters.parameters;
                int tlen = tlist.Length;
                for (int i = 0; i < tlen; i++)
                {
                    var item = tlist[i];
                    playableAnimator.StateController.Params.AddParameter(item);
                }
            }
        }
        public void AddStates(PlayableAnimator playableAnimator)
        {
            AddParameters(playableAnimator);

            for (int i = 0; i < stateLayers.Length; i++)
            {
                stateLayers[i].AddLayer(playableAnimator, i);
                stateLayers[i].AddStates(playableAnimator, i);
            }
        }
    }
}

