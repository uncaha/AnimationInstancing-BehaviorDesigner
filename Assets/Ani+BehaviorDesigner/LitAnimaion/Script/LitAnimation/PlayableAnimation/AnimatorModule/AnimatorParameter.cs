using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AniPlayable.Module
{
    public class AnimatorParameter : AnimatorModule
    {
        public string name { get { return parameter.name; } }
        public int index {get;private set;}
        public AnimatorControllerParameterType type { get { return parameter.type; } }
        public int intValue = 0;
        public float floatValue = 0;
        public bool boolValue = false;
        protected AssetParameter.Parameter parameter;
        public AnimatorParameter(AssetParameter.Parameter pData,int pIndex)
        {
            parameter = pData;
            index = pIndex;

            intValue = pData.defaultInt;
            floatValue = pData.defaultFloat;
            boolValue = pData.defaultBool;
        }
    }
}