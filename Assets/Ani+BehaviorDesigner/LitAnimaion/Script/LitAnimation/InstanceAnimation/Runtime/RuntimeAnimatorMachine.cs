
using System;
using System.Collections.Generic;
namespace AniPlayable.InstanceAnimation
{
    
    public class RuntimeAnimatorMachine: Node
    {
        public readonly int layerIndex;
        public readonly int index;
        public readonly string defaultName;
        public readonly int defaultHashName;
        public readonly int machineHashName;

        public RuntimeAnimatorState defaultState { get; protected set; }
        AnimationStateMachineInfo machineInfo;
        private RuntimeAnimatorState[] stateList;
        private Dictionary<int,int> hashMap = new Dictionary<int, int>();
        private int stateLength = 0;
        public RuntimeAnimatorMachine(AnimationStateMachineInfo pInfo)
        {
            if(pInfo == null) return;
            machineInfo = pInfo;
            layerIndex = machineInfo.layerIndex;
            index = machineInfo.index;
            defaultName = machineInfo.defaultName;
            defaultHashName = machineInfo.defaultHashName;
            machineHashName = machineInfo.machineHashName;
        }
        public override void InitNode(AnimationInstancing pAnimator)
        {
            var tpam = parameters;
            stateLength = machineInfo.stateInfos.Count;
            stateList = new RuntimeAnimatorState[stateLength];
            for (int i = 0; i < stateLength; i++)
            {
                var item = machineInfo.stateInfos[i];
                var tstate = new RuntimeAnimatorState(item) { parameters = tpam };
                tstate.InitNode(pAnimator);

                if (tstate.nameHash == machineInfo.defaultHashName)
                {
                    defaultState = tstate;
                }
                stateList[i] = tstate;

                hashMap.Add(tstate.nameHash,i);
            }

        }

        public RuntimeAnimatorState this[int pIndex]
        {
            get
            {
                if(pIndex < 0 || pIndex >= stateLength) return null;
                return stateList[pIndex];
            }
        }

        #region get
        public int GetStateIndex(int pHash)
        {
            if (hashMap.TryGetValue(pHash, out int tindex))
            {
                return tindex;
            }
            return -1;
        }
        public RuntimeAnimatorState GetState(string pStateName)
        {
            return GetState(pStateName.GetHashCode());
        }
        public RuntimeAnimatorState GetState(int pHash)
        {
            if (hashMap.TryGetValue(pHash, out int tindex))
            {
                return this[tindex];
            }
            return null;
        }
        #endregion

    }
}