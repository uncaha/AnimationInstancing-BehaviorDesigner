using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace AniPlayable
{
    public class PlayableAnimator : MonoBehaviour
    {
        #region serlizeddata
        [SerializeField]private AssetStateController _assetStateController;
        [SerializeField]protected string defaultState;
        [SerializeField]protected bool isAutoPlay = false;
        [SerializeField]protected float sampleIntervalp;
        public AnimatorUpdateMode mode = AnimatorUpdateMode.Normal;
        #endregion

        #region control
        public bool isPlaying { get; protected set; }
        public bool isPause { get; protected set; }
        private UpdateObject updateObject;
        #endregion
        private PlayableGraph m_Graph;
        private Animator m_Animator;
        private Playable m_OutputPlayable;
        private PlayableStateController m_StateController;
        public PlayableStateController StateController {get { return m_StateController; } }

        private bool m_IsInitialized = false;

        #region Init Method
        private void Awake()
        {
            Init();
            if(isAutoPlay)
            {
                Play(defaultState);
            }
        }

        public void Init()
        {
            if (m_IsInitialized) return;
            Initialize();
            if (_assetStateController != null)
            {
                _assetStateController.AddStates(this);
            }
        }

        private void Initialize()
        {
            if (m_IsInitialized) return;
            m_Animator = GetComponent<Animator>();
            if(m_Animator != null)
            {
                Destroy(m_Animator);
            }
            m_Animator = gameObject.AddComponent<Animator>();

            m_Graph = PlayableGraph.Create();
            m_Graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
            m_StateController = new PlayableStateController(m_Graph);

            var template = new PlayableAmimatorDriver();
            template.Initialize(m_Graph, m_StateController);
            m_OutputPlayable = ScriptPlayable<PlayableAmimatorDriver>.Create(m_Graph, template, 1);

            AnimationPlayableUtilities.Play(m_Animator, m_OutputPlayable, m_Graph);

            m_Graph.Stop();

            updateObject = new UpdateObject(this,UpdateGraph,mode);
            PlayableUpdateManager.Reg(updateObject);
            m_IsInitialized = true;
        }

        float sampleTimer = 0;
        public void UpdateGraph(float dt)
        {
            if(!isPlaying || isPause) return;
            sampleTimer += dt;
            if(sampleTimer > sampleIntervalp)
            {
                m_Graph.Evaluate(sampleTimer);
                sampleTimer = 0;
            }
        }

        private void OnEnable()
        {
            m_Animator.enabled = true;
        }

        private void OnDisable()
        {
            m_Animator.enabled = false;
        }

        private void OnDestroy()
        {
            PlayableUpdateManager.UnReg(updateObject);
            m_Graph.Destroy();
        }
        #endregion
        
        void GoPlaying()
        {
            isPlaying = true;
            isPause = false;
        }
        public void Play()
        {
            GoPlaying();
            m_OutputPlayable.Play();
        }

        public void Pause()
        {
            isPause = false;
            m_OutputPlayable.Pause();
        }

        public void Play(string stateName, int layer = 0)
        {
            GoPlaying();
            m_StateController.EnableState(stateName, layer);
        }

        public void PlayInFixedTime(string stateName, float fixedTime, int layer = 0)
        {
            GoPlaying();
            m_StateController.EnableState(stateName, fixedTime, layer);
        }

        public void Crossfade(string stateName, float normalnizeTime, int layer = 0)
        {
            GoPlaying();
            m_StateController.Crossfade(stateName, normalnizeTime, true, layer);
        }

        public void CrossfadeInFixedTime(string stateName, float fixedTime, int layer = 0)
        {
            GoPlaying();
            m_StateController.Crossfade(stateName, fixedTime, false, layer);
        }

        public PlayableStateController.StateInfo AddState(AnimationClip clip, string stateName, string groupName = null, int layer = 0)
        {
            if (clip == null)
            {
                Debug.LogWarningFormat("clip :{0} is null!", stateName);
                return null;
            }
            AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(m_Graph, clip);
            if (!clip.isLooping || clip.wrapMode == WrapMode.Once)
            {
                clipPlayable.SetDuration(clip.length);
            }
            return m_StateController.AddState(stateName, clipPlayable, clip, groupName, layer);
        }

        public PlayableStateController.StateInfo AddBlendTree(PlayableStateController.BlendTreeConfig[] configs, string paramName, string stateName = null, string groupName = null, int layer = 0)
        {
            if (configs == null) return null;
            for (int i = 0; i < configs.Length; i++)
            {
                AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(m_Graph, configs[i].clip);
                configs[i].playable = clipPlayable;
            }
            Playable playable = Playable.Create(m_Graph, 1);
            return m_StateController.AddBlendTree(stateName, playable, configs, paramName, groupName, layer);
        }

        public void SetBool(string pKey, bool pValue)
        {
            StateController.Params.SetBool(pKey, pValue);
        }

        public void SetInt(string pKey, int pValue)
        {
            StateController.Params.SetInt(pKey,pValue);
        }

        public void SetFloat(string pKey, float pValue)
        {
            StateController.Params.SetFloat(pKey,pValue);
        }

        public bool GetBool(string pKey)
        {
            return StateController.Params.GetBool(pKey);
        }

        public int GetInt(string pKey)
        {
            return StateController.Params.GetInt(pKey);
        }

        public float GetFloat(string pKey)
        {
            return StateController.Params.GetFloat(pKey);
        }
    }
}

