using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AniPlayable.Module;
using System;
namespace AniPlayable.InstanceAnimation
{
    public class AnimationInstancing : MonoBehaviour
    {
        public class LodInfo
        {
            public int lodLevel;
            public SkinnedMeshRenderer[] skinnedMeshRenderer;
            public MeshRenderer[] meshRenderer;
            public MeshFilter[] meshFilter;
            public VertexCache[] vertexCacheList;
            public MaterialBlock[] materialBlockList;
        }
        private Animator animator = null;

        #region serialized field
        public string aniFilename = "";
        public BoundingSphere boundingSpere;
        public bool visible { get; set; }
        public AnimationInstancing parentInstance { get; set; }
        public float playSpeed = 1.0f;
        public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
        public bool receiveShadow;
        public bool applyRootMotion = false;
        public bool autoPlay = false;
        public string defaultState = "";
        [Range(1, 4)]
        public int bonePerVertex = 4;
        #endregion

        #region runtime
        public PlayableAnimatorParameter Params { get; private set; }
        public RuntimeAnimatorLayerGroup Layers { get; private set; }
        [NonSerialized] public Transform worldTransform;
        [NonSerialized] public int layer;
        float speedParameter = 1.0f, cacheParameter = 1.0f;
        [NonSerialized]public float curProcess = 0;
        WrapMode wrapMode;
        public WrapMode Mode
        {
            get { return wrapMode; }
            set { wrapMode = value; }
        }
        public bool IsLoop() { return Mode == WrapMode.Loop; }
        public bool IsPause() { return speedParameter == 0.0f; }

        private RuntimeAnimatorState cureState ;

        [NonSerialized] public float curFrame;

        [NonSerialized] public float preAniFrame;

        [NonSerialized] public int aniIndex = -1;

        [NonSerialized] public int preAniIndex = -1;

        [NonSerialized] public int aniTextureIndex = -1;
        int preAniTextureIndex = -1;
        float transitionDuration = 0.0f;
        bool isInTransition = false;
        float transitionTimer = 0.0f;

        [NonSerialized] public float transitionProgress = 0.0f;
        private int eventIndex = -1;

        public AnimationInfo[] aniInfo;
        private ComparerHash comparer;
        private AnimationInfo searchInfo;
        private AnimationEvent aniEvent = null;

        [NonSerialized]
        public LodInfo[] lodInfo;
        private float lodCalculateFrequency = 0.5f;
        private float lodFrequencyCount = 0.0f;
        [NonSerialized]
        public int lodLevel;
        private Transform[] allTransforms;
        private bool isMeshRender = false;
        [NonSerialized]
        private List<AnimationInstancing> listAttachment;
        #endregion

        #region Init Method
        private bool m_IsInitialized = false;
        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (m_IsInitialized) return;
            Initialize();
            if (autoPlay)
            {
                Play();
            }

        }
        private void Initialize()
        {
            if (m_IsInitialized) return;

            if (!AnimationInstancingMgr.Instance.UseInstancing)
            {
                gameObject.SetActive(false);
                return;
            }
            Params = new PlayableAnimatorParameter();
            worldTransform = transform;
            animator = GetComponent<Animator>();
            boundingSpere = new BoundingSphere(new Vector3(0, 0, 0), 1.0f);
            listAttachment = new List<AnimationInstancing>();
            layer = gameObject.layer;

            switch (QualitySettings.skinWeights)
            {
                case SkinWeights.TwoBones:
                    bonePerVertex = bonePerVertex > 2 ? 2 : bonePerVertex;
                    break;
                case SkinWeights.OneBone:
                    bonePerVertex = 1;
                    break;
            }

            UnityEngine.Profiling.Profiler.BeginSample("Calculate lod");
            LODGroup lod = GetComponent<LODGroup>();
            if (lod != null)
            {
                lodInfo = new LodInfo[lod.lodCount];
                LOD[] lods = lod.GetLODs();
                for (int i = 0; i != lods.Length; ++i)
                {
                    if (lods[i].renderers == null)
                    {
                        continue;
                    }

                    LodInfo info = new LodInfo();
                    info.lodLevel = i;
                    info.vertexCacheList = new VertexCache[lods[i].renderers.Length];
                    info.materialBlockList = new MaterialBlock[info.vertexCacheList.Length];
                    List<SkinnedMeshRenderer> listSkinnedMeshRenderer = new List<SkinnedMeshRenderer>();
                    List<MeshRenderer> listMeshRenderer = new List<MeshRenderer>();
                    foreach (var render in lods[i].renderers)
                    {
                        if (render is SkinnedMeshRenderer)
                            listSkinnedMeshRenderer.Add((SkinnedMeshRenderer)render);
                        if (render is MeshRenderer)
                            listMeshRenderer.Add((MeshRenderer)render);
                    }
                    info.skinnedMeshRenderer = listSkinnedMeshRenderer.ToArray();
                    info.meshRenderer = listMeshRenderer.ToArray();
                    //todo, to make sure whether the MeshRenderer can be in the LOD.
                    info.meshFilter = null;
                    for (int j = 0; j != lods[i].renderers.Length; ++j)
                    {
                        lods[i].renderers[j].enabled = false;
                    }
                    lodInfo[i] = info;
                }
            }
            else
            {
                lodInfo = new LodInfo[1];
                LodInfo info = new LodInfo();
                info.lodLevel = 0;
                info.skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
                info.meshRenderer = GetComponentsInChildren<MeshRenderer>();
                info.meshFilter = GetComponentsInChildren<MeshFilter>();
                info.vertexCacheList = new VertexCache[info.skinnedMeshRenderer.Length + info.meshRenderer.Length];
                info.materialBlockList = new MaterialBlock[info.vertexCacheList.Length];
                lodInfo[0] = info;

                for (int j = 0; j != info.meshRenderer.Length; ++j)
                {
                    info.meshRenderer[j].enabled = false;
                }
                for (int j = 0; j != info.skinnedMeshRenderer.Length; ++j)
                {
                    info.skinnedMeshRenderer[j].enabled = false;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();

            if (AnimationInstancingMgr.Instance.UseInstancing
                && animator != null)
            {
                animator.enabled = false;
            }
            visible = true;
            CalcBoundingSphere();
            AnimationInstancingMgr.Instance.AddBoundingSphere(this);
            AnimationInstancingMgr.Instance.AddInstance(gameObject);

            m_IsInitialized = true;
        }
        private void OnDestroy()
        {
            if (!AnimationInstancingMgr.IsDestroy())
            {
                AnimationInstancingMgr.Instance.RemoveInstance(this);
            }
            if (parentInstance != null)
            {
                parentInstance.Deattach(this);
                parentInstance = null;
            }
        }

        private void OnEnable()
        {
            playSpeed = 1.0f;
            visible = true;
            if (listAttachment != null)
            {
                for (int i = 0; i != listAttachment.Count; ++i)
                {
                    listAttachment[i].gameObject.SetActive(true);
                }
            }
        }

        private void OnDisable()
        {
            playSpeed = 0.0f;
            visible = false;
            if (listAttachment != null)
            {
                for (int i = 0; i != listAttachment.Count; ++i)
                {
                    listAttachment[i].gameObject.SetActive(false);
                }
            }
        }

        public bool InitializeAnimation()
        {
            if (String.IsNullOrEmpty(aniFilename))
            {
                Debug.LogError("The aniFilename is NULL. Please select the aniFilename first.");
            }

            isMeshRender = false;
            if (lodInfo[0].skinnedMeshRenderer.Length == 0)
            {
                // This is only a MeshRenderer, it has no animations.
                isMeshRender = true;
                AnimationInstancingMgr.Instance.AddMeshVertex(aniFilename,
                    lodInfo,
                    null,
                    null,
                    bonePerVertex);
                return true;
            }

            searchInfo = new AnimationInfo();
            comparer = new ComparerHash();

            AnimationManager.InstanceAnimationInfo info = AnimationManager.Instance.FindAnimationInfo(aniFilename, this);
            if (info != null)
            {
                aniInfo = info.listAniInfo.ToArray();
                PrepareParams(info.paramList);
                PrepareLayer(info.layerList);
                Prepare(aniInfo,info.extraBoneInfo);
            }
            
            return true;
        }

        private void PrepareParams(List<AssetParameter.Parameter> pPamList)
        {
            for (int i = 0; i < pPamList.Count; i++)
            {
                Params.AddParameter(pPamList[i]);
            }
        }

        private void PrepareLayer(List<AnimationLayerInfo> pLayerList)
        {
            Layers = new RuntimeAnimatorLayerGroup(pLayerList) { parameters = Params };
            Layers.InitNode(this);
        }

        public void Prepare(AnimationInfo[] pinfos, ExtraBoneInfo extraBoneInfo)
        {
            //extraBoneInfo = extraBoneInfo;
            List<Matrix4x4> bindPose = new List<Matrix4x4>(150);
            // to optimize, MergeBone don't need to call every time
            Transform[] bones = RuntimeHelper.MergeBone(lodInfo[0].skinnedMeshRenderer, bindPose);
            allTransforms = bones;

            if (extraBoneInfo != null)
            {
                List<Transform> list = new List<Transform>();
                list.AddRange(bones);
                Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
                for (int i = 0; i != extraBoneInfo.extraBone.Length; ++i)
                {
                    for (int j = 0; j != transforms.Length; ++j)
                    {
                        if (extraBoneInfo.extraBone[i] == transforms[j].name)
                        {
                            list.Add(transforms[j]);
                        }
                    }
                    bindPose.Add(extraBoneInfo.extraBindPose[i]);
                }
                allTransforms = list.ToArray();
            }


            AnimationInstancingMgr.Instance.AddMeshVertex(aniFilename,
                lodInfo,
                allTransforms,
                bindPose,
                bonePerVertex);

            foreach (var lod in lodInfo)
            {
                foreach (var cache in lod.vertexCacheList)
                {
                    cache.shadowcastingMode = shadowCastingMode;
                    cache.receiveShadow = receiveShadow;
                    cache.layer = layer;
                }
            }

            Destroy(GetComponent<Animator>());

        }
        #endregion

        #region tool
        private void CalcBoundingSphere()
        {
            UnityEngine.Profiling.Profiler.BeginSample("CalcBoundingSphere()");
            Bounds bound = new Bounds(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            LodInfo info = lodInfo[0];
            for (int i = 0; i != info.meshRenderer.Length; ++i)
            {
                MeshRenderer meshRenderer = info.meshRenderer[i];
                bound.Encapsulate(meshRenderer.bounds);
            }
            for (int i = 0; i != info.skinnedMeshRenderer.Length; ++i)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = info.skinnedMeshRenderer[i];
                bound.Encapsulate(skinnedMeshRenderer.bounds);
            }
            float radius = bound.size.x > bound.size.y ? bound.size.x : bound.size.y;
            radius = radius > bound.size.z ? radius : bound.size.z;
            boundingSpere.radius = radius;
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public AnimationInfo GetCurrentAnimationInfo()
        {
            if (aniInfo != null && 0 <= aniIndex && aniIndex < aniInfo.Length)
            {
                return aniInfo[aniIndex];
            }
            return null;
        }

        public AnimationInfo GetPreAnimationInfo()
        {
            if (aniInfo != null && 0 <= preAniIndex && preAniIndex < aniInfo.Length)
            {
                return aniInfo[preAniIndex];
            }
            return null;
        }

        public int FindAnimationInfo(int hash)
        {
            if (aniInfo == null)
                return -1;
            searchInfo.animationNameHash = hash;
            return Array.BinarySearch<AnimationInfo>(aniInfo, searchInfo, comparer);
        }
        #endregion

        #region control

        #region control State
        private RuntimeAnimatorState GetState(int pHashState, int pLayer,int pMechine = 0)
        {
            RuntimeAnimatorLayer tlayer = Layers[pLayer];
            if (tlayer == null)
            {
                Debug.LogError("Can't found layer by index " + pLayer);
                return null;
            }
            if (tlayer[pMechine] == null)
            {
                Debug.LogError(string.Format("the layer is not have machine. layer = {0} pMechine = {1}" ,pLayer, pMechine));
                return null;
            }

            return pHashState == 0 ? tlayer[pMechine].defaultState : tlayer[pMechine].GetState(pHashState);
        }

        public RuntimeAnimatorState GetStateByIndex(int pIndex, int pLayer, int pMechine = 0)
        {
            RuntimeAnimatorLayer tlayer = Layers[pLayer];
            if (tlayer == null)
            {
                Debug.LogError("Can't found layer by index " + pLayer);
                return null;
            }
            if (tlayer[pMechine] == null)
            {
                Debug.LogError(string.Format("the layer is not have machine. layer = {0} pMechine = {1}" ,pLayer, pMechine));
                return null;
            }
            return tlayer[pMechine][pIndex];
        }

        public void Play()
        {
            PlayState(GetState(0,0));
        }
        public void Play(string pState, int pLayer = 0,int pMechine = 0)
        {
            Play(pState.GetHashCode(), pLayer,pMechine);
        }
        public void Play(int pHashState, int pLayer = 0,int pMechine = 0)
        {
            PlayState(GetState(pHashState,pLayer,pMechine));
        }

        public void PlayState(RuntimeAnimatorState pState)
        {
            if (pState == null)
            {
                Debug.LogError("Can't found tstate by hash " + pState);
                return;
            }
            if(pState.motionIndex >= 0)
            {
                cureState = pState;
                PlayAnimation(pState.motionIndex);
            }  
        }

        public void CrossFade(string stateName, float duration,int pLayer = 0,int pMechine = 0)
        {
            CrossFade(stateName.GetHashCode(),duration,pLayer,pMechine);
        }

        public void CrossFade(int stateHash, float duration ,int pLayer = 0,int pMechine = 0)
        {
            CrossFade(GetState(stateHash, pLayer,pMechine), duration);
        }

        public void CrossFade(RuntimeAnimatorState pState, float duration)
        {
            if (pState == null)
            {
                Debug.LogError("Can't found tstate by hash " + pState);
                return;
            }
            if(pState.motionIndex >= 0)
            {
                cureState = pState;
                CrossFadeAnimation(pState.motionIndex,duration);
            }  
        }
        #endregion

        #region control animation
        private void PlayAnimation(string name)
        {
            int hash = name.GetHashCode();
            int index = FindAnimationInfo(hash);
            PlayAnimation(index);
        }

        private void PlayAnimation(int animationIndex)
        {
            if (aniInfo == null)
            {
                return;
            }
            if (animationIndex == aniIndex && !IsPause())
            {
                return;
            }
            
            curProcess = 0;
            transitionDuration = 0.0f;
            transitionProgress = 1.0f;
            isInTransition = false;
            Debug.Assert(animationIndex < aniInfo.Length);
            if (0 <= animationIndex && animationIndex < aniInfo.Length)
            {
                preAniIndex = aniIndex;
                aniIndex = animationIndex;
                preAniFrame = (float)(int)(curFrame + 0.5f);
                curFrame = 0.0f;
                eventIndex = -1;
                preAniTextureIndex = aniTextureIndex;
                aniTextureIndex = aniInfo[aniIndex].textureIndex;
                wrapMode = aniInfo[aniIndex].wrapMode;
                speedParameter = 1.0f;
            }
            else
            {
                Debug.LogWarning("The requested animation index is out of the count.");
                return;
            }
            RefreshAttachmentAnimation(aniTextureIndex);
        }

        private void CrossFadeAnimation(string animationName, float duration)
        {
            int hash = animationName.GetHashCode();
            int index = FindAnimationInfo(hash);
            CrossFadeAnimation(index, duration);
        }

        private void CrossFadeAnimation(int animationIndex, float duration)
        {
            PlayAnimation(animationIndex);
            if (duration > 0.0f)
            {
                isInTransition = true;
                transitionTimer = 0.0f;
                transitionProgress = 0.0f;
            }
            else
            {
                transitionProgress = 1.0f;
            }
            transitionDuration = duration;
        }

        public void Pause()
        {
            cacheParameter = speedParameter;
            speedParameter = 0.0f;
        }

        public void Resume()
        {
            speedParameter = cacheParameter;
        }

        public void Stop()
        {
            aniIndex = -1;
            preAniIndex = -1;
            eventIndex = -1;
            curFrame = 0.0f;
            curProcess = 0;

            cureState = null;
        }

        public bool IsPlaying()
        {
            return aniIndex >= 0 || isMeshRender;
        }

        public bool IsReady()
        {
            return aniInfo != null;
        }
        #endregion

        #endregion

        #region  update
        public void UpdateAnimation()
        {
            if (aniInfo == null || IsPause())
                return;
            
            if (isInTransition)
            {
                transitionTimer += Time.deltaTime;
                float weight = transitionTimer / transitionDuration;
                transitionProgress = Mathf.Min(weight, 1.0f);
                if (transitionProgress >= 1.0f)
                {
                    isInTransition = false;
                    preAniIndex = -1;
                    preAniFrame = -1;
                }
            }
            float speed = playSpeed * speedParameter;
            curFrame += speed * Time.deltaTime * aniInfo[aniIndex].fps;
            int totalFrame = aniInfo[aniIndex].totalFrame;
            curProcess = Mathf.Clamp01(curFrame / totalFrame);
            switch (wrapMode)
            {
                case WrapMode.Loop:
                    {
                        if (curFrame < 0f)
                            curFrame += (totalFrame - 1);
                        else if (curFrame > totalFrame - 1)
                        {
                            curProcess = 1;
                            curFrame -= (totalFrame - 1);
                        } 
                        break;
                    }
                case WrapMode.PingPong:
                    {
                        if (curFrame < 0f)
                        {
                            speedParameter = Mathf.Abs(speedParameter);
                            curFrame = Mathf.Abs(curFrame);
                        }
                        else if (curFrame > totalFrame - 1)
                        {
                            speedParameter = -Mathf.Abs(speedParameter);
                            curFrame = 2 * (totalFrame - 1) - curFrame;
                            curProcess = 1;
                        }
                        break;
                    }
                case WrapMode.Default:
                case WrapMode.Once:
                    {
                        if (curFrame < 0f || curFrame > totalFrame - 1.0f)
                        {
                            Pause();
                            curProcess = 1;
                        }
                        break;
                    }
            }

            curFrame = Mathf.Clamp(curFrame, 0f, totalFrame - 1);
            for (int i = 0; i != listAttachment.Count; ++i)
            {
                AnimationInstancing attachment = listAttachment[i];
                attachment.transform.position = transform.position;
                attachment.transform.rotation = transform.rotation;
            }
            UpdateAnimationEvent();
            UpdateState();
        }
        
        void UpdateState()
        {
            if (cureState == null || cureState.motionIndex != aniIndex) return;
            AnimationInfo info = aniInfo[aniIndex];
            
            var ttrans = cureState.CheckTransition(curProcess);
            if (ttrans != null)
            {
                RuntimeAnimatorState tstate = null;
                if(ttrans.destinationType == AssetTransitions.DestinationType.state)
                {
                    tstate = GetState(ttrans.destinationHashName,cureState.layerIndex,cureState.machineIndex);
                }
                else if(ttrans.destinationType == AssetTransitions.DestinationType.stateMachine)
                {
                    if(ttrans.destinationIndex == -1)
                    {
                        ttrans.destinationIndex = Layers[cureState.layerIndex].GetMachineIndex(ttrans.destinationHashName);
                    }
                    tstate = GetState(0,cureState.layerIndex,ttrans.destinationIndex);
                }
                
                if (tstate == null)
                {
                    Pause();
                }
                else
                {
                    CrossFade(tstate, ttrans.duration);
                }
                
            }
        }

        public void UpdateLod(Vector3 cameraPosition)
        {
            lodFrequencyCount += Time.deltaTime;
            if (lodFrequencyCount > lodCalculateFrequency)
            {
                float sqrLength = (cameraPosition - worldTransform.position).sqrMagnitude;
                if (sqrLength < 50.0f)
                    lodLevel = 0;
                else if (sqrLength < 500.0f)
                    lodLevel = 1;
                else
                    lodLevel = 2;
                lodFrequencyCount = 0.0f;
                lodLevel = Mathf.Clamp(lodLevel, 0, lodInfo.Length - 1);
            }
        }

        private void UpdateAnimationEvent()
        {
            AnimationInfo info = GetCurrentAnimationInfo();
            if (info == null)
                return;
            if (info.eventList.Count == 0)
                return;

            if (aniEvent == null)
            {
                float time = curFrame / info.fps;
                for (int i = eventIndex >= 0 ? eventIndex : 0; i < info.eventList.Count; ++i)
                {
                    if (info.eventList[i].time > time)
                    {
                        aniEvent = info.eventList[i];
                        eventIndex = i;
                        break;
                    }
                }
            }

            if (aniEvent != null)
            {
                float time = curFrame / info.fps;
                if (aniEvent.time <= time)
                {
                    gameObject.SendMessage(aniEvent.function, aniEvent);
                    aniEvent = null;
                }
            }
        }
        #endregion

        #region attach
        public void Attach(string boneName, AnimationInstancing attachment)
        {
            int index = -1;
            int hashBone = boneName.GetHashCode();
            for (int i = 0; i != allTransforms.Length; ++i)
            {
                if (allTransforms[i].name.GetHashCode() == hashBone)
                {
                    index = i;
                    break;
                }
            }
            Debug.Assert(index >= 0);
            if (index < 0)
            {
                Debug.LogError("Can't find the bone.");
                return;
            }
            if (attachment.lodInfo[0].meshRenderer.Length == 0 && attachment.lodInfo[0].skinnedMeshRenderer.Length == 0)
            {
                Debug.LogError("The attachment doesn't have a Renderer");
                return;
            }

            attachment.parentInstance = this;
            VertexCache parentCache = AnimationInstancingMgr.Instance.FindVertexCache(lodInfo[0].skinnedMeshRenderer[0].name.GetHashCode());
            listAttachment.Add(attachment);

            int nameCode = boneName.GetHashCode();
            nameCode += attachment.lodInfo[0].meshRenderer.Length > 0 ? attachment.lodInfo[0].meshRenderer[0].name.GetHashCode() : 0;
            if (attachment.lodInfo[0].meshRenderer.Length == 0)
            {
                //todo, to support the attachment that has skinnedMeshRenderer;
                int skinnedMeshRenderCount = attachment.lodInfo[0].skinnedMeshRenderer.Length;
                nameCode += skinnedMeshRenderCount > 0 ? attachment.lodInfo[0].skinnedMeshRenderer[0].name.GetHashCode() : 0;
            }
            VertexCache cache = AnimationInstancingMgr.Instance.FindVertexCache(nameCode);
            // if we can reuse the VertexCache, we don't need to create one
            if (cache != null)
            {
                cache.boneTextureIndex = parentCache.boneTextureIndex;
                return;
            }

            AnimationInstancingMgr.Instance.AddMeshVertex(attachment.aniFilename,
                        attachment.lodInfo,
                        null,
                        null,
                        attachment.bonePerVertex,
                        boneName);

            for (int i = 0; i != attachment.lodInfo.Length; ++i)
            {
                LodInfo info = attachment.lodInfo[i];
                for (int j = 0; j != info.meshRenderer.Length; ++j)
                {
                    cache = info.vertexCacheList[info.skinnedMeshRenderer.Length + j];
                    Debug.Assert(cache != null);
                    if (cache == null)
                    {
                        Debug.LogError("Can't find the VertexCache.");
                        continue;
                    }
                    Debug.Assert(cache.boneTextureIndex < 0 || cache.boneIndex[0].x != index);

                    AnimationInstancingMgr.Instance.BindAttachment(parentCache, cache, info.meshFilter[j].sharedMesh, index);
                    AnimationInstancingMgr.Instance.SetupAdditionalData(cache);
                    cache.boneTextureIndex = parentCache.boneTextureIndex;
                }
            }
        }


        public void Deattach(AnimationInstancing attachment)
        {
            attachment.visible = false;
            attachment.parentInstance = null;
            RefreshAttachmentAnimation(-1);
            listAttachment.Remove(attachment);
        }

        public int GetAnimationCount()
        {
            return aniInfo != null ? aniInfo.Length : 0;
        }

        private void RefreshAttachmentAnimation(int index)
        {
            for (int k = 0; k != listAttachment.Count; ++k)
            {
                AnimationInstancing attachment = listAttachment[k];
                //attachment.aniIndex = aniIndex;
                for (int i = 0; i != attachment.lodInfo.Length; ++i)
                {
                    LodInfo info = attachment.lodInfo[i];
                    for (int j = 0; j != info.meshRenderer.Length; ++j)
                    {
                        //MeshRenderer render = info.meshRenderer[j];
                        VertexCache cache = info.vertexCacheList[info.skinnedMeshRenderer.Length + j];
                        cache.boneTextureIndex = index;
                    }
                }
            }
        }
        #endregion

        #region params
        public void SetBool(string pKey, bool pValue)
        {
            Params.SetBool(pKey, pValue);
        }

        public void SetInt(string pKey, int pValue)
        {
            Params.SetInt(pKey, pValue);
        }

        public void SetFloat(string pKey, float pValue)
        {
            Params.SetFloat(pKey, pValue);
        }

        public bool GetBool(string pKey)
        {
            return Params.GetBool(pKey);
        }

        public int GetInt(string pKey)
        {
            return Params.GetInt(pKey);
        }

        public float GetFloat(string pKey)
        {
            return Params.GetFloat(pKey);
        }
        #endregion
    }
}