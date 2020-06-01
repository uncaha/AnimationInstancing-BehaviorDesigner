using UnityEngine;
using UnityEngine.Playables;

namespace AniPlayable
{
    public class PlayableAmimatorDriver : PlayableBehaviour
    {
        private PlayableGraph m_Graph;
        private Playable m_Playable;
        private PlayableStateController m_StateController;

        public Playable playable { get { return m_Playable; } }

        public void Initialize(PlayableGraph graph, PlayableStateController ctrl)
        {
            m_Graph = graph;
            m_StateController = ctrl;
        }

        public override void OnPlayableCreate(Playable playable)
        {
            m_Playable = playable;
            m_StateController.SetPlayableOutput(0, 0, playable);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            
        }
        
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            m_StateController.Update(info.deltaTime);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {

        }
    }
}
