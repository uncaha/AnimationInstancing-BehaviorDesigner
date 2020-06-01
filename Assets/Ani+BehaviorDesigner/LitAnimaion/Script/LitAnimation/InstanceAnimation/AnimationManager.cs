using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AniPlayable.Module;
namespace AniPlayable.InstanceAnimation
{
  public class AnimationManager : SingletonMonoBehaviour<AnimationManager>
    {
        // A request to create animation info, because we use async method
        struct CreateAnimationRequest 
        {
            public GameObject prefab;
            public AnimationInstancing instance;
        }
        // A container to storage all animations info within game object
        public class InstanceAnimationInfo 
        {
            public List<AssetParameter.Parameter> paramList;
            public List<AnimationLayerInfo> layerList;
            public List<AnimationInfo> listAniInfo;
            public ExtraBoneInfo extraBoneInfo;
        }

        private Dictionary<string, InstanceAnimationInfo> m_animationInfo;

        private AssetBundle m_mainBundle;
        bool m_useBundle = false;

        protected override void InitMgr()
        {
            m_animationInfo = new Dictionary<string, InstanceAnimationInfo>();
        }

        public InstanceAnimationInfo FindAnimationInfo(string pFileName, AnimationInstancing instance)
        {
            Debug.Assert(pFileName != null);
            InstanceAnimationInfo info = null;
            if (m_animationInfo.TryGetValue(pFileName, out info))
            {
                return info;
            }
            
            // #if UNITY_IPHONE || UNITY_ANDROID
            //             Debug.Assert(m_useBundle);
            // 			if (m_mainBundle == null)
            //             	Debug.LogError("You should call LoadAnimationAssetBundle first.");
            // #endif
            //             if (m_useBundle)
            //             {
            //                 CreateAnimationRequest request = new CreateAnimationRequest();
            //                 request.prefab = prefab;
            //                 request.instance = instance;
            //                 if (m_mainBundle != null)
            //                 {
            //                     StartCoroutine(LoadAnimationInfoFromAssetBundle(request));
            //                 }
            //                 else
            //                 {
            //                     m_requestList.Add(request);
            //                 }
            //                 return null;
            //             }
            //             else
            return CreateAnimationInfoFromFile(pFileName);
        }

        public IEnumerator LoadAnimationAssetBundle(string path)
        {
            m_useBundle = true;
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
            yield return request;

			if (request.assetBundle != null)
            {
                Debug.LogFormat("Load the AB {0} successed.", path);
                m_mainBundle = request.assetBundle;
            }
        }

        public void UnloadAnimationAssetBundle()
        {
            if (m_mainBundle != null)
            {
                m_mainBundle.Unload(false);
            }
        }
        private InstanceAnimationInfo CreateAnimationInfoFromFile(string pFileName)
        {
            Debug.Assert(!string.IsNullOrEmpty(pFileName));
            string path = "AnimationTexture/" + pFileName;
            TextAsset tdata = Resources.Load<TextAsset>(path);
            if(tdata == null)
            {
                Debug.LogError("Can't load asset.path = " + path);
                return null;
            }
            BinaryReader reader = new BinaryReader(new MemoryStream(tdata.bytes));
            InstanceAnimationInfo info = new InstanceAnimationInfo();
            info.paramList = ReadParameters(reader);
            info.layerList = ReadLayers(reader);
            info.listAniInfo = ReadAnimationInfo(reader);
            info.extraBoneInfo = ReadExtraBoneInfo(reader);
            m_animationInfo.Add(pFileName, info);
            AnimationInstancingMgr.Instance.ImportAnimationTexture(pFileName, reader);
            reader.Close();
            Resources.UnloadAsset(tdata);
            return info;
        }

        private List<AssetParameter.Parameter> ReadParameters(BinaryReader reader)
        {
            List<AssetParameter.Parameter> ret = new List<AssetParameter.Parameter>();
            int tcount = reader.ReadInt32();
            for (int i = 0; i < tcount; i++)
            {
                AssetParameter.Parameter tpamdata = new AssetParameter.Parameter();
                tpamdata.ReadFromFile(reader);
                ret.Add(tpamdata);
            }
            return ret;
        }

        private List<AnimationLayerInfo> ReadLayers(BinaryReader reader)
        {
            List<AnimationLayerInfo> ret = new List<AnimationLayerInfo>();
            int tcount = reader.ReadInt32();
  
            for (int j = 0; j < tcount; j++)
            {
                var item = new AnimationLayerInfo();
                item.ReadFromFile(reader);
                ret.Add(item);
            }
            return ret;
        }

        private List<AnimationInfo> ReadAnimationInfo(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            List<AnimationInfo>  listInfo = new List<AnimationInfo>();
            for (int i = 0; i != count; ++i)
            {
                AnimationInfo info = new AnimationInfo();
                //info.animationNameHash = reader.ReadInt32();
                info.animationName = reader.ReadString();
                info.animationNameHash = info.animationName.GetHashCode();
                info.animationIndex = reader.ReadInt32();
                info.textureIndex = reader.ReadInt32();
                info.totalFrame = reader.ReadInt32();
                info.fps = reader.ReadInt32();
                info.rootMotion = reader.ReadBoolean();
                info.wrapMode = (WrapMode)reader.ReadInt32();
                if (info.rootMotion)
                {
                    info.velocity = new Vector3[info.totalFrame];
                    info.angularVelocity = new Vector3[info.totalFrame];
                    for (int j = 0; j != info.totalFrame; ++j)
                    {
                        info.velocity[j].x = reader.ReadSingle();
                        info.velocity[j].y = reader.ReadSingle();
                        info.velocity[j].z = reader.ReadSingle();

                        info.angularVelocity[j].x = reader.ReadSingle();
                        info.angularVelocity[j].y = reader.ReadSingle();
                        info.angularVelocity[j].z = reader.ReadSingle();
                    }
                }
                int evtCount = reader.ReadInt32();
                info.eventList = new List<AnimationEvent>();
                for (int j = 0; j != evtCount; ++j)
                {
                    AnimationEvent evt = new AnimationEvent();
                    evt.function = reader.ReadString();
                    evt.floatParameter = reader.ReadSingle();
                    evt.intParameter = reader.ReadInt32();
                    evt.stringParameter = reader.ReadString();
                    evt.time = reader.ReadSingle();
                    evt.objectParameter = reader.ReadString();
                    info.eventList.Add(evt);
                }

                listInfo.Add(info);
            }
            listInfo.Sort(new ComparerHash());
            return listInfo;
        }

        private ExtraBoneInfo ReadExtraBoneInfo(BinaryReader reader)
        {
            ExtraBoneInfo info = null;
            if (reader.ReadBoolean())
            {
                info = new ExtraBoneInfo();
                int count = reader.ReadInt32();
                info.extraBone = new string[count];
                info.extraBindPose = new Matrix4x4[count];
                for (int i = 0; i != info.extraBone.Length; ++i)
                {
                    info.extraBone[i] = reader.ReadString();
                }
                for (int i = 0; i != info.extraBindPose.Length; ++i)
                {
                    for (int j = 0; j != 16; ++j)
                    {
                        info.extraBindPose[i][j] = reader.ReadSingle();
                    }
                }
            }
            return info;
        }
    }
}