using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AniPlayable
{
    [CreateAssetMenu(fileName = "AssetCondition", menuName = "PlayableAnimator/Condition", order = 0)]
    public class AssetCondition : ScriptableObject
    {
        public enum AnimatorConditionMode
        {
            If = 1,
            IfNot = 2,
            Greater = 3,
            Less = 4,
            Equals = 6,
            NotEqual = 7
        }
        [System.Serializable]
        public class Condition
        {
            public AnimatorConditionMode mode;
            public string parameter;
            public float threshold;
#if UNITY_EDITOR
            public void CopyData(ref UnityEditor.Animations.AnimatorCondition pSource)
            {
                mode = (AnimatorConditionMode)pSource.mode;
                parameter = pSource.parameter;
                threshold = pSource.threshold;
            }

            public void WriteToFile(System.IO.BinaryWriter pWriter)
            {
                pWriter.Write((int)mode);
                pWriter.Write(parameter);
                pWriter.Write(threshold);
            }
#endif
            public void ReadFromFile(System.IO.BinaryReader pReader)
            {
                mode = (AnimatorConditionMode)pReader.ReadInt32();
                parameter = pReader.ReadString();
                threshold = pReader.ReadSingle();
            }
        }

        public Condition condition;
    }
}