using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AniPlayable.Module
{
    public class AnimatorTransition : AnimatorModule
    {
        
        public readonly int Index ;
        public readonly AssetTransitions.DestinationType destinationType;
        public readonly string  destinationName ;
        public readonly int destinationHashName ;
        public readonly float duration;
        public readonly float exitTime ;
        public readonly int ownerMachineIndex = 0;

        public int destinationIndex = -1;

        AnimatorCondition[] conditions;
        int conditionLength = 0;
        public AnimatorTransition(PlayableAnimatorParameter parms,AssetTransitions.Transtions pData, int pIndex)
        {
            AssetTransitions.Transtions transtion = pData;
            Index = pIndex;
            duration = transtion.duration;
            exitTime = transtion.exitTime;
            destinationName = transtion.destinationName;
            destinationType = transtion.destinationType;

            
            if(transtion.conditions != null && transtion.conditions.Length > 0)
            {
                conditionLength = transtion.conditions.Length;
                conditions = new AnimatorCondition[conditionLength];
                for (int i = 0; i < conditions.Length; i++)
                {
                    conditions[i] = new AnimatorCondition(parms,transtion.conditions[i],i);
                }
            }

            destinationHashName = destinationName.GetHashCode();
            ownerMachineIndex = transtion.ownerMachineIndex;
        }

        public bool CheckCondition()
        {
            if(conditionLength == 0) return true;
            bool isOk = true;
            for (int i = 0; i < conditionLength; i++)
            {
                var item = conditions[i];
                if(!item.CheckCondition())
                {
                    isOk = false;
                    break;
                }
            }
            return isOk;
        }

    }
}