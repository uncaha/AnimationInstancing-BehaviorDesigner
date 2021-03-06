﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AniPlayable
{
    [CreateAssetMenu(fileName = "StateLayer", menuName = "PlayableAnimator/StateLayer", order = 3)]
    public class AssetStateLayer : ScriptableObject
    {
        public AssetStateGroup[] stateGroups;
        public AvatarMask avatarMask;
        public bool isAdditive;
        public bool _IKPass;
        //public bool sync;
        public bool timing;
        public int sourceLayerIndex = -1;   // -1代表无同步层
        // todo: editor编辑

        /// <summary>
        /// 在控制器中添加该层的所有动画
        /// </summary>
        /// <param name="playableAnimator"></param>
        /// <param name="layer"></param>
        public void AddStates(PlayableAnimator playableAnimator, int layer)
        {
            if (stateGroups != null)
            {
                for (int i = 0; i < stateGroups.Length; i++)
                {
                    stateGroups[i].AddStates(playableAnimator, layer);
                }
            }
        }

        /// <summary>
        /// 在控制器中添加层
        /// </summary>
        /// <param name="playableAnimator"></param>
        /// <param name="layer"></param>
        public void AddLayer(PlayableAnimator playableAnimator, int layer)
        {
            var ctrl = playableAnimator.StateController;
            if (layer > 0)  // 0 为自动创建的默认层
            {
                ctrl.AddLayer(layer);
            }

            var layerInstance = ctrl.GetLayer(layer);
            if (layerInstance != null)
            {
                layerInstance.IKPass = _IKPass;
                layerInstance.SyncLayerIndex = sourceLayerIndex;
                layerInstance.IsAdditive = isAdditive;
                layerInstance.timing = timing;

                if (avatarMask != null)
                {
                    layerInstance.AvatarMask = avatarMask;
                }
            }
        }
    }
}

