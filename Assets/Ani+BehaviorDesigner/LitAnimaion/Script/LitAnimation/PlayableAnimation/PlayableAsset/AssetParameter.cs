using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AniPlayable
{
    [CreateAssetMenu(fileName = "AssetParameter", menuName = "PlayableAnimator/Parameter", order = 0)]
    public class AssetParameter : ScriptableObject
    {
        [System.Serializable]
        public class Parameter
        {
            public string name;
            public int nameHash;
            public AnimatorControllerParameterType type ;
            public float defaultFloat ;
            public int defaultInt ;
            public bool defaultBool ;

            public void CopyData(AnimatorControllerParameter des)
            {
                name = des.name;
                nameHash = des.nameHash;
                type = des.type;
                defaultFloat = des.defaultFloat;
                defaultInt = des.defaultInt;
                defaultBool = des.defaultBool;
            }

            public void WriteToFile(System.IO.BinaryWriter pWriter)
            {
                pWriter.Write(name);
                pWriter.Write(nameHash);
                pWriter.Write((int)type);
                pWriter.Write(defaultFloat);
                pWriter.Write(defaultInt);
                pWriter.Write(defaultBool);
            }

            public void ReadFromFile(System.IO.BinaryReader pReader)
            {
                name = pReader.ReadString();
                nameHash = pReader.ReadInt32();
                type = (AnimatorControllerParameterType)pReader.ReadInt32();
                defaultFloat = pReader.ReadSingle();
                defaultInt = pReader.ReadInt32();
                defaultBool = pReader.ReadBoolean();
            }
        }
        public Parameter[] parameters;
    }
}