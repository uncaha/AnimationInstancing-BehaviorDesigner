using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AniPlayable
{
    public interface IPlayableAnimatorNode
    {
        void SetPlayableOutput(int outputPort, int inputPort, Playable inputNode);
    }

    public interface IPlayableBlendTree
    {
        void Sort();
        void SetBlendTreeType();
    }


    public interface IAnimatorParameter
    {

    }
}

