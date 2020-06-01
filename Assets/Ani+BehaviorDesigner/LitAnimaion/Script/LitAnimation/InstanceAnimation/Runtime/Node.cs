namespace AniPlayable.InstanceAnimation
{
    public abstract class Node
    {
        public PlayableAnimatorParameter parameters;
        public abstract void InitNode(AnimationInstancing pAnimator);
    }
}