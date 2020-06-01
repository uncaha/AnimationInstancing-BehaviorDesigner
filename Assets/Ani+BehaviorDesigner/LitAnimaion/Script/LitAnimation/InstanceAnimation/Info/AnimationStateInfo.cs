using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AniPlayable.Module;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
namespace AniPlayable.InstanceAnimation
{
    public class AnimationStateInfo
    {
        public int layerIndex = 0;
        public int machineIndex = 0;
        public int index = 0;
        public string name;
        public int hashName;
        public float speed = 1;
        public string motionName;
        public List<AssetTransitions.Transtions> transtionList = new List<AssetTransitions.Transtions>();
#if UNITY_EDITOR
        public void SetData(UnityEditor.Animations.AnimatorState pState)
        {
            name = pState.name;
            hashName = pState.nameHash;
            speed = pState.speed;

            AnimationClip clip = pState.motion as AnimationClip;
            motionName = clip != null ? clip.name: "";

            transtionList.Clear();
            AnimatorStateTransition[] transs = pState.transitions;
            for (int k = 0; k < transs.Length; k++)
            {
                AnimatorStateTransition transdata = transs[k];
                AssetTransitions.Transtions ttranstion = new AssetTransitions.Transtions(){ownerMachineIndex = machineIndex};
                ttranstion.CopyData(transdata);
                transtionList.Add(ttranstion);
            }
        }

        public void WriteToFile(System.IO.BinaryWriter pWriter)
        {
            pWriter.Write(layerIndex);
            pWriter.Write(machineIndex);
            pWriter.Write(index);
            pWriter.Write(name);
            pWriter.Write(speed);
            pWriter.Write(motionName);

            pWriter.Write(transtionList.Count);
            for (int i = 0; i < transtionList.Count; i++)
            {
                AssetTransitions.Transtions item = transtionList[i];
                item.WriteToFile(pWriter);
            }

        }
#endif
        public void ReadFromFile(System.IO.BinaryReader pReader)
        {
            layerIndex = pReader.ReadInt32();
            machineIndex = pReader.ReadInt32();
            index = pReader.ReadInt32();
            name = pReader.ReadString();
            speed = pReader.ReadSingle();
            motionName = pReader.ReadString();

            hashName = name.GetHashCode();

            int tlen = pReader.ReadInt32();
            for (int i = 0; i < tlen; i++)
            {
                var item = new AssetTransitions.Transtions();
                item.ReadFromFile(pReader);
                transtionList.Add(item);
            }
        }
    }
}