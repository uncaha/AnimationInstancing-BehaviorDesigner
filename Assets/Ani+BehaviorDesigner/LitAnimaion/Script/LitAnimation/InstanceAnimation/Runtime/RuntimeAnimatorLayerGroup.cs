using UnityEngine;
using System;
using System.Collections.Generic;

namespace AniPlayable.InstanceAnimation
{
    public class RuntimeAnimatorLayerGroup: Node
    {   
        public List<AnimationLayerInfo> layerListInfo {get; private set;}
        private RuntimeAnimatorLayer[] LayerList;
        private int layerLength = 0;
        public RuntimeAnimatorLayerGroup(List<AnimationLayerInfo> pInfo)
        {
            layerListInfo = pInfo;
        }

        public override void InitNode(AnimationInstancing pAnimator)
        {
            var tpam = parameters;
            layerLength = layerListInfo.Count;
            LayerList = new RuntimeAnimatorLayer[layerLength];
            for (int i = 0; i < layerLength; i++)
            {
                var item = layerListInfo[i];
                var trtlayer = new RuntimeAnimatorLayer(item){parameters = tpam};
                trtlayer.InitNode(pAnimator);
                LayerList[i] = trtlayer;
            }
        }

        public RuntimeAnimatorLayer this[int index]
        {
            get
            {
                if (index < 0 || index >= layerLength) return null;
                return LayerList[index];
            }
        }
    }
}