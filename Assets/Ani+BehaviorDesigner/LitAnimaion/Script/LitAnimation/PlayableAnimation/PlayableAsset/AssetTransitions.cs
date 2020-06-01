using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AniPlayable
{
    [CreateAssetMenu(fileName = "AssetTransitions", menuName = "PlayableAnimator/Transition", order = 0)]
    public class AssetTransitions : ScriptableObject
    {
        public enum DestinationType
        {
            none = 0,
            state,
            stateMachine,
        }
        [System.Serializable]
        public class Transtions
        {
            public int ownerMachineIndex = 0;
            public DestinationType destinationType = DestinationType.none;
            public string destinationName = "";
            public float duration;
            public float exitTime;
            public AssetCondition.Condition[] conditions;
#if UNITY_EDITOR
            public void CopyData(UnityEditor.Animations.AnimatorStateTransition pSource)
            {
                if (pSource == null) return;
                
                if(pSource.destinationState != null)
                {
                    destinationType = DestinationType.state;
                    destinationName = pSource.destinationState.name;
                }else if(pSource.destinationStateMachine != null)
                {
                    destinationType = DestinationType.stateMachine;
                    destinationName = pSource.destinationStateMachine.name;
                }
                
                duration = pSource.duration;
                exitTime = pSource.exitTime;

                var tcondions = pSource.conditions;
                conditions = new AssetCondition.Condition[tcondions.Length];
                for (int i = 0; i < conditions.Length; i++)
                {
                    AssetCondition.Condition item = new AssetCondition.Condition();
                    item.CopyData(ref tcondions[i]);
                    conditions[i] = item;
                }
            }

            public void WriteToFile(System.IO.BinaryWriter pWriter)
            {
                pWriter.Write(ownerMachineIndex);
                pWriter.Write((int)destinationType);
                pWriter.Write(destinationName);
                pWriter.Write(duration);
                pWriter.Write(exitTime);

                int tlen = conditions == null ? 0 : conditions.Length;
                pWriter.Write(tlen);
                if (conditions != null)
                {
                    for (int i = 0; i < conditions.Length; i++)
                    {
                        AssetCondition.Condition item = conditions[i];
                        item.WriteToFile(pWriter);
                    }
                }

            }
#endif
            public void ReadFromFile(System.IO.BinaryReader pReader)
            {
                ownerMachineIndex = pReader.ReadInt32();
                destinationType = (DestinationType)pReader.ReadInt32();
                destinationName = pReader.ReadString();
                duration = pReader.ReadSingle();
                exitTime = pReader.ReadSingle();

                int tlen = pReader.ReadInt32();
                conditions = new AssetCondition.Condition[tlen];
                for (int i = 0; i < tlen; i++)
                {
                    conditions[i] = new AssetCondition.Condition();
                    conditions[i].ReadFromFile(pReader);
                }
            }
        }
        public Transtions data;
    }
}