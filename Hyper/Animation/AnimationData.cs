namespace Hyper.Animation;

public class AnimationData
{
    public bool IsAnimationRunning { get; set; }
    
    public int AnimationIndex { get; set; }
    
    public float AnimationTotalTime { get; set; }
    
    public float AnimationCurrentTime { get; set; }
    
    public bool IsAnimationLooping { get; set; }
    
    public float AnimationSpeed { get; set; }
}