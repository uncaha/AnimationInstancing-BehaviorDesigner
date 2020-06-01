using UnityEngine;
namespace AniPlayable.InstanceAnimation
{
    public class GenerateOjbectInfo
    {
        public Matrix4x4 worldMatrix;
        public int nameCode;
        public float animationTime;
        public int stateName;
        public int frameIndex;
        public int boneListIndex = -1;
        public Matrix4x4[] boneMatrix;
    }
}