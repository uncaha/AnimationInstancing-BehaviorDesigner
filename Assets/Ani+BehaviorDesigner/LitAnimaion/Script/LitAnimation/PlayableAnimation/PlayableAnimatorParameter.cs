using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AniPlayable.Module;
namespace AniPlayable
{
    public class PlayableAnimatorParameter : IAnimatorParameter
    {
        

        private Dictionary<string, AnimatorParameter> m_Params = new Dictionary<string,AnimatorParameter>(5);
        private List<AnimatorParameter> m_ParamList = new List<AnimatorParameter>(5);

        public AnimatorParameter AddParameter(AssetParameter.Parameter pam)
        {
            if (!m_Params.ContainsKey(pam.name))
            {
                AnimatorParameter item = new AnimatorParameter(pam, m_Params.Count);
                m_Params.Add(item.name, item);
                m_ParamList.Add(item);
            }
            return m_Params[pam.name];
        }

        public AnimatorParameter AddBool(string name)
        {
            if(m_Params.ContainsKey(name))
            {
                Debug.LogError("the Key is aready in list.key = " + name);
                return m_Params[name];
            }
            AssetParameter.Parameter tdata = new AssetParameter.Parameter();
            tdata.name = name;
            tdata.type = AnimatorControllerParameterType.Bool;
            return AddParameter(tdata);
        }

        public AnimatorParameter AddFloat(string name)
        {
            if(m_Params.ContainsKey(name))
            {
                Debug.LogError("the Key is aready in list.key = " + name);
                return m_Params[name];
            }
            AssetParameter.Parameter tdata = new AssetParameter.Parameter();
            tdata.name = name;
            tdata.type = AnimatorControllerParameterType.Float;
            return AddParameter(tdata);
        }

        public AnimatorParameter AddInt(string name)
        {
            if (m_Params.ContainsKey(name))
            {
                Debug.LogError("the Key is aready in list.key = " + name);
                return m_Params[name];
            }
            AssetParameter.Parameter tdata = new AssetParameter.Parameter();
            tdata.name = name;
            tdata.type = AnimatorControllerParameterType.Int;
            return AddParameter(tdata);
        }

        public AnimatorParameter GetParameter(string pName)
        {
            if (!m_Params.ContainsKey(pName)) return null;
            return m_Params[pName];
        }
        public AnimatorParameter GetParameter(int id)
        {
            if(id < 0 || id >= m_ParamList.Count) return null;
            return m_ParamList[id];
        }

        public bool GetBool(string name)
        {
            bool ret = default(bool);
            if (m_Params.ContainsKey(name))
            {
                ret = m_Params[name].boolValue;
            }
            return ret;
        }

        public bool GetBool(int id)
        {
            if(id < 0 || id >= m_ParamList.Count) return default(bool);
            return m_ParamList[id].boolValue;
        }

        public float GetFloat(string name)
        {
            float ret = default(float);
            if (m_Params.ContainsKey(name))
            {
                ret = m_Params[name].floatValue;
            }
            return ret;
        }

        public float GetFloat(int id)
        {
            if(id < 0 || id >= m_ParamList.Count) return default(float);
            return m_ParamList[id].floatValue;
        }

        public int GetInt(string name)
        {
            int ret = default(int);
            if (m_Params.ContainsKey(name))
            {
                ret = m_Params[name].intValue;
            }
            return ret;
        }

        public int GetInt(int id)
        {
            if (id < 0 || id >= m_ParamList.Count) return default(int);
            return m_ParamList[id].intValue;
        }

        public void SetBool(string name, bool val)
        {
            if (m_Params.ContainsKey(name))
            {
                m_Params[name].boolValue = val;
            }
        }

        public void SetBool(int id, bool val)
        {
            if (id < 0 || id >= m_ParamList.Count) return;
            m_ParamList[id].boolValue = val;
        }

        public void SetFloat(string name, float val)
        {
            if (m_Params.ContainsKey(name))
            {
                m_Params[name].floatValue = val;
            }
        }

        public void SetFloat(int id, float val)
        {
            if (id < 0 || id >= m_ParamList.Count) return;
            m_ParamList[id].floatValue = val;
        }

        public void SetInt(string name, int val)
        {
            if (m_Params.ContainsKey(name))
            {
                m_Params[name].intValue = val;
            }
        }

        public void SetInt(int id, int val)
        {
            if (id < 0 || id >= m_ParamList.Count) return;
            m_ParamList[id].intValue = val;
        }

        public bool Contains(string name)
        {
            return m_Params.ContainsKey(name);
        }
    }
}


