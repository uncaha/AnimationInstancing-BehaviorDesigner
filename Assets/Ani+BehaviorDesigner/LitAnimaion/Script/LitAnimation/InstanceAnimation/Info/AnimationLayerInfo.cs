using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AniPlayable.InstanceAnimation
{
    public class AnimationLayerInfo
    {
        public string name;
        public int index;
        public List<AnimationStateMachineInfo> machines = new List<AnimationStateMachineInfo>();

        public AnimationStateMachineInfo defaultMachine;
        #if UNITY_EDITOR
        public void WriteToFile(System.IO.BinaryWriter pWriter)
        {
            pWriter.Write(name);
            pWriter.Write(index);

            pWriter.Write(machines.Count);
            for (int i = 0; i < machines.Count; i++)
            {
                var item = machines[i];
                item.WriteToFile(pWriter);
            }

        }
#endif

        public void ReadFromFile(System.IO.BinaryReader pReader)
        {
            name = pReader.ReadString();
            index = pReader.ReadInt32();

            int tlen = pReader.ReadInt32();
            for (int i = 0; i < tlen; i++)
            {
                var tmachine = new AnimationStateMachineInfo();
                tmachine.ReadFromFile(pReader);
                machines.Add(tmachine);
            }
            defaultMachine = machines.Count > 0 ? machines[0] : null;
        }
    }
}