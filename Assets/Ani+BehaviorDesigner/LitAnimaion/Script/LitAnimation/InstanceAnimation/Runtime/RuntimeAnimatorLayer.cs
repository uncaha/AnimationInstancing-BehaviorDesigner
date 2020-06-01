using UnityEngine;
using System;
using System.Collections.Generic;
namespace AniPlayable.InstanceAnimation
{
    public class RuntimeAnimatorLayer : Node
    {
        public string name { get { return layerInfo.name; } }
        public int index { get { return layerInfo.index; } }
        public RuntimeAnimatorMachine defaultStateMachine {get ; private set;}
        
        AnimationLayerInfo layerInfo;
        int machineCount = 0;
        private RuntimeAnimatorMachine[] machineList;
        private Dictionary<int,int> hashMap = new Dictionary<int, int>();
        public RuntimeAnimatorLayer(AnimationLayerInfo pInfo)
        {
            layerInfo = pInfo;
        }
        public override void InitNode(AnimationInstancing pAnimator)
        {
            var tpam = parameters;
            machineCount = layerInfo.machines.Count;
            machineList = new RuntimeAnimatorMachine[machineCount];
            for (int i = 0; i < machineCount; i++)
            {   
                var item = layerInfo.machines[i];
                var tmachine = new RuntimeAnimatorMachine(item){parameters = tpam};
                tmachine.InitNode(pAnimator);
                machineList[i] = tmachine;
                hashMap.Add(tmachine.machineHashName,i);
            }
            if(machineCount > 0)
                defaultStateMachine = machineList[0];
        }

        public RuntimeAnimatorMachine this[int index]
        {
            get
            {
                if (index < 0 || index >= machineCount) return null;
                return machineList[index];
            }
        }

        public int GetMachineIndex(int pHashName)
        {
            if(hashMap.TryGetValue(pHashName,out int ret))
            {
                return ret;
            }
            return -1;
        }
    }
}