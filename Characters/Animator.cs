using System.Diagnostics;
using Assimp;
using Quaternion = Assimp.Quaternion;

namespace Character;
/// <summary>
/// This class is responsible for animating the model.
/// </summary>
public class Animator
{
    private int _animationIndex = 0;

    private bool _isAnimationRunning = false;

    private readonly Stopwatch _stopwatch;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Animator"/> class.
    /// </summary>
    public Animator()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    /// <summary>
    /// Gets the transforms of the bones.
    /// </summary>
    /// <param name="model">Model from which we get the transforms.</param>
    /// <param name="meshIndex">Index of the mesh of the model.</param>
    /// <returns>The transforms of the bones.</returns>
    public Matrix4x4[] GetBoneTransforms(Scene model, int meshIndex)
    {
        if (!_isAnimationRunning)
            return Enumerable.Repeat(model.RootNode.Transform, model.Meshes[meshIndex].BoneCount).ToArray();

        var dict = new Dictionary<string, Matrix4x4>();
        GetBoneDict(model.RootNode, Matrix4x4.Identity, ref dict);

        return model.Meshes[meshIndex].Bones.Select(bone =>
            bone.OffsetMatrix * dict[bone.Name]).ToArray();
    }

    /// <summary>
    /// Animates the model.
    /// </summary>
    /// <param name="model">The model that is animated.</param>
    public void Animate(Scene model)
    {
        if (!_isAnimationRunning) return;

        var time = GetCurrentTime(model);
        var animation = model.Animations[_animationIndex];

        foreach (var channel in animation.NodeAnimationChannels)
        {
            var nodeTransform = CalculateNodeTransform(channel, (float)time);
            var node = model.RootNode.FindNode(channel.NodeName);
            node.Transform = nodeTransform;
        }
    }

    /// <summary>
    /// Resets the animation.
    /// </summary>
    public void Reset()
    {
        _stopwatch.Reset();
        _isAnimationRunning = false;
    }

    /// <summary>
    /// Starts playing the animation.
    /// </summary>
    /// <param name="index">Index of the animation what is played.</param>
    public void Play(int index)
    {
        if (_animationIndex == index && _isAnimationRunning) return;

        Restart(index);
    }

    private void Restart(int index)
    {
        _animationIndex = index;
        _isAnimationRunning = true;
        _stopwatch.Restart();
    }

    private double GetCurrentTime(Scene model)
    {
        var seconds = _stopwatch.ElapsedMilliseconds / 1000.0;
        var ticks = seconds * model.Animations[_animationIndex].TicksPerSecond / 2;
        return ticks % model.Animations[_animationIndex].DurationInTicks;
    }
    
    private void GetBoneDict(Node node, Matrix4x4 parentTransform, ref Dictionary<string, Matrix4x4> bones)
    {
        var transform = node.Transform * parentTransform;
        bones.Add(node.Name, transform);

        foreach (var child in node.Children)
        {
            GetBoneDict(child, transform, ref bones);
        }
    }

    private Matrix4x4 CalculateNodeTransform(NodeAnimationChannel channel, float time)
    {
        Matrix4x4 transform = Matrix4x4.Identity;
        transform *= GetScale(channel, time);
        transform *= GetRotation(channel, time);
        transform *= GetTranslation(channel, time);

        return transform;
    }

    private static Matrix4x4 GetTranslation(NodeAnimationChannel channel, float time)
    {
        if (!channel.HasPositionKeys) return Matrix4x4.Identity;

        Vector3D position = channel.PositionKeys[0].Value;

        for (int i = 0; i < channel.PositionKeys.Count - 1; i++)
        {
            if (time < channel.PositionKeys[i + 1].Time)
            {
                double factor = (time - channel.PositionKeys[i].Time) / (channel.PositionKeys[i + 1].Time - channel.PositionKeys[i].Time);
                position = Interpolate(channel.PositionKeys[i].Value, channel.PositionKeys[i + 1].Value, (float)factor);
                break;
            }
        }

        return Matrix4x4.FromTranslation(position);
    }

    private static Matrix4x4 GetRotation(NodeAnimationChannel channel, float time)
    {
        if (!channel.HasRotationKeys) return Matrix4x4.Identity;

        Quaternion rotation = channel.RotationKeys[0].Value;

        for (int i = 0; i < channel.RotationKeys.Count - 1; i++)
        {
            if (time < channel.RotationKeys[i + 1].Time)
            {
                double factor = (time - channel.RotationKeys[i].Time) / (channel.RotationKeys[i + 1].Time - channel.RotationKeys[i].Time);
                rotation = Interpolate(channel.RotationKeys[i].Value, channel.RotationKeys[i + 1].Value, (float)factor);
                break;
            }
        }

        return rotation.GetMatrix();
    }

    private static Matrix4x4 GetScale(NodeAnimationChannel channel, float time)
    {
        if (!channel.HasScalingKeys) return Matrix4x4.Identity;

        Vector3D scale = channel.ScalingKeys[0].Value;

        for (int i = 0; i < channel.ScalingKeys.Count - 1; i++)
        {
            if (time < channel.ScalingKeys[i + 1].Time)
            {
                double factor = (time - channel.ScalingKeys[i].Time) / (channel.ScalingKeys[i + 1].Time - channel.ScalingKeys[i].Time);
                scale = Interpolate(channel.ScalingKeys[i].Value, channel.ScalingKeys[i + 1].Value, (float)factor);
                break;
            }
        }

        return Matrix4x4.FromScaling(scale);
    }

    private static Vector3D Interpolate(Vector3D previousValue, Vector3D nextValue, float factor)
    {
        return Lerp(previousValue, nextValue, factor);
    }

    private static Quaternion Interpolate(Quaternion previousValue, Quaternion nextValue, float factor)
    {
        return Quaternion.Slerp(previousValue, nextValue, factor);
    }

    private static Vector3D Lerp(Vector3D a, Vector3D b, float t)
    {
        return new Vector3D(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y), a.Z + t * (b.Z - a.Z));
    }
}