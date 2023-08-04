using System.Diagnostics;
using Assimp;
using Quaternion = Assimp.Quaternion;

namespace Hyper.Animation;

public class Animator
{
    private int _animationIndex;
    public bool IsAnimationRunning { get; set; } = true;
    private readonly Stopwatch _stopwatch;

    public Animator()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }
    
    public Matrix4x4[] GetBones(Assimp.Scene model, int meshIndex)
    {
        var dict = new Dictionary<string, Matrix4x4>();
        GetBones2(model.RootNode, Matrix4x4.Identity, ref dict);
        
        return model.Meshes[meshIndex].Bones.Select(bone =>
            bone.OffsetMatrix * dict[bone.Name]).ToArray();
    }

    private void GetBones2(Node node, Matrix4x4 parentTransform, ref Dictionary<string, Matrix4x4> bones)
    {
        var transform = node.Transform * parentTransform;
        bones.Add(node.Name, transform);
        
        foreach (var child in node.Children)
        {
            GetBones2(child, transform, ref bones);
        }
    }

    public void Animate(Assimp.Scene model)
    {
        var time = GetCurrentTime(model);
        var animation = model.Animations[_animationIndex];

        foreach (var channel in animation.NodeAnimationChannels)
        {
            var nodeTransform = CalculateNodeTransform(channel, (float)time);
            var node = model.RootNode.FindNode(channel.NodeName);
            node.Transform = nodeTransform;
        }
    }

    private double GetCurrentTime(Assimp.Scene model)
    {
        var seconds = _stopwatch.ElapsedMilliseconds / 1000.0;
        var ticks = seconds * model.Animations[_animationIndex].TicksPerSecond;
        return ticks % model.Animations[_animationIndex].DurationInTicks;
    }
    
    private Matrix4x4 CalculateNodeTransform(NodeAnimationChannel channel, float time)
    {
        Matrix4x4 transform = Matrix4x4.Identity;
        transform *= GetScale(channel, time);
        transform *= GetRotation(channel, time);
        transform *= GetTranslation(channel, time);

        return transform;
    }

    private Matrix4x4 GetTranslation(NodeAnimationChannel channel, float time)
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

    private Matrix4x4 GetRotation(NodeAnimationChannel channel, float time)
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

    private Matrix4x4 GetScale(NodeAnimationChannel channel, float time)
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

    private Vector3D Interpolate(Vector3D previousValue, Vector3D nextValue, float factor)
    {
        return Lerp(previousValue, nextValue, factor);
    }

    private Quaternion Interpolate(Quaternion previousValue, Quaternion nextValue, float factor)
    {
        return Quaternion.Slerp(previousValue, nextValue, factor);
    }
    
    private static Vector3D Lerp(Vector3D a, Vector3D b, float t)
    {
        return new Vector3D(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y), a.Z + t * (b.Z - a.Z));
    }
}