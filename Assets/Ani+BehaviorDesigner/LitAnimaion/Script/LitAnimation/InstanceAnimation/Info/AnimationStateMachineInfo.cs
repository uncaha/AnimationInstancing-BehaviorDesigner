using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AniPlayable.InstanceAnimation
{
    public class AnimationStateMachineInfo
    {
        public int layerIndex;
        public int index;
        public string machineName;
        public string defaultName;
        public int defaultHashName;
        public int machineHashName;
        public AnimationStateInfo defaultState;
        public List<AnimationStateInfo> stateInfos = new List<AnimationStateInfo>();

#if UNITY_EDITOR
        public void WriteToFile(System.IO.BinaryWriter pWriter)
        {
            pWriter.Write(layerIndex);
            pWriter.Write(index);
            pWriter.Write(machineName);
            pWriter.Write(defaultName);

            pWriter.Write(stateInfos.Count);
            for (int i = 0; i < stateInfos.Count; i++)
            {
                var item = stateInfos[i];
                item.WriteToFile(pWriter);
            }

        }
#endif

        public void ReadFromFile(System.IO.BinaryReader pReader)
        {
            layerIndex = pReader.ReadInt32();
            index = pReader.ReadInt32();
            machineName = pReader.ReadString();
            defaultName = pReader.ReadString();
            
            int tlen = pReader.ReadInt32();
            for (int i = 0; i < tlen; i++)
            {
                var tstate = new AnimationStateInfo();
                tstate.ReadFromFile(pReader);
                if(tstate.hashName == defaultHashName)
                {
                    defaultState = tstate;
                }
                stateInfos.Add(tstate);
            }
            
            machineHashName = machineName.GetHashCode();
            defaultHashName = defaultName.GetHashCode();
        }
    }
}