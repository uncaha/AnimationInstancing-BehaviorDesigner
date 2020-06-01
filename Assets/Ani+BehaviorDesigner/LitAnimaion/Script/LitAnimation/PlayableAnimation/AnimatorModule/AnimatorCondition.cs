using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AniPlayable.Module
{
    public class AnimatorCondition: AnimatorModule
    {
        public readonly int Index ;

        public AssetCondition.AnimatorConditionMode mode;
        public float threshold;
        protected AssetCondition.Condition conditionData;
        protected AnimatorParameter parameterData;
        protected System.Func<bool> conditionDelgate;
        public AnimatorCondition(PlayableAnimatorParameter parms,AssetCondition.Condition pData, int pIndex)
        {
            conditionData = pData;
            Index = pIndex;

            mode = conditionData.mode;
            threshold = conditionData.threshold;

            parameterData = parms.GetParameter(conditionData.parameter);

            switch (parameterData.type)
            {
                case AnimatorControllerParameterType.Int:
                    conditionDelgate = GetConditionDelgate_Int();
                    break;
                case AnimatorControllerParameterType.Float:
                    conditionDelgate = GetConditionDelgate_Float();
                    break;
                case AnimatorControllerParameterType.Bool:
                    conditionDelgate = GetConditionDelgate_Bool();
                    break;
                case AnimatorControllerParameterType.Trigger:
                    conditionDelgate = GetConditionDelgate_Trigger();
                    break;
                default:
                    conditionDelgate = CheckConditionDelgate_Null;
                    break;
            }
        }

        #region check
        public bool CheckCondition()
        {
            return conditionDelgate();
        }
        System.Func<bool> GetConditionDelgate_Bool()
        {
            if(mode == AssetCondition.AnimatorConditionMode.If)
            {
                return Check_If;
            }
            else if(mode == AssetCondition.AnimatorConditionMode.IfNot)
            {
                return Check_IfNot;
            }

            return CheckConditionDelgate_Null;
        }

        System.Func<bool> GetConditionDelgate_Int()
        {
            switch (mode)
            {
                case AssetCondition.AnimatorConditionMode.Greater:
                    return Check_GreaterInt;
                case AssetCondition.AnimatorConditionMode.Less:
                    return Check_LessInt;
                case AssetCondition.AnimatorConditionMode.Equals:
                    return Check_Equals;
                case AssetCondition.AnimatorConditionMode.NotEqual:
                    return Check_NotEqual;
                default:
                    return CheckConditionDelgate_Null;
            }
        }
        System.Func<bool> GetConditionDelgate_Float()
        {
            switch (mode)
            {
                case AssetCondition.AnimatorConditionMode.Greater:
                    return Check_GreaterFloat;
                case AssetCondition.AnimatorConditionMode.Less:
                    return Check_LessFloat;
                default:
                    return CheckConditionDelgate_Null;
            }
        }
        System.Func<bool> GetConditionDelgate_Trigger()
        {
            return Check_If;
        }

        #endregion

        #region condition Mode

        bool CheckConditionDelgate_Null()
        {
            return true;
        }
        bool Check_If()
        {
            return parameterData.boolValue;
        }
        bool Check_IfNot()
        {
            return !parameterData.boolValue;
        }
        bool Check_GreaterFloat()
        {
            return parameterData.floatValue > threshold;
        }
        bool Check_LessFloat()
        {
            return parameterData.floatValue < threshold;
        }

        bool Check_GreaterInt()
        {
            return parameterData.intValue > threshold;
        }
        bool Check_LessInt()
        {
            return parameterData.intValue < threshold;
        }

        bool Check_Equals()
        {
            return (int)threshold == parameterData.intValue;
        }
        bool Check_NotEqual()
        {
            return (int)threshold != parameterData.intValue;
        }
        #endregion
    }
}