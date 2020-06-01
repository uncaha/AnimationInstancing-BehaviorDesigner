
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
namespace AniPlayable.InstanceAnimation
{
    public class InstanceData
    {
        public List<Matrix4x4[]>[] worldMatrix;
        public List<float[]>[] frameIndex;
        public List<float[]>[] preFrameIndex;
        public List<float[]>[] transitionProgress;
    }

    public class InstancingPackage
    {
        public Material[] material;
        public int animationTextureIndex = 0;
        public int subMeshCount = 1;
        public int instancingCount;
        public int size;
        public MaterialPropertyBlock propertyBlock;
    }
    public class MaterialBlock
    {
        public InstanceData instanceData;
        public int[] runtimePackageIndex;
        // array[index base on texture][package index]
        public List<InstancingPackage>[] packageList;
    }
    public class VertexCache
    {
        public int nameCode;
        public Mesh mesh = null;
        public Dictionary<int, MaterialBlock> instanceBlockList;
        public Vector4[] weight;
        public Vector4[] boneIndex;
        public Material[] materials = null;
        public Matrix4x4[] bindPose;
        public Transform[] bonePose;
        public int boneTextureIndex = -1;

        // these are temporary, should be moved to InstancingPackage
        public ShadowCastingMode shadowcastingMode;
        public bool receiveShadow;
        public int layer;
    }

    public class AnimationTexture
    {
        public string name { get; set; }
        public Texture2D[] boneTexture { get; set; }
        public int blockWidth { get; set; }
        public int blockHeight { get; set; }
    }
}