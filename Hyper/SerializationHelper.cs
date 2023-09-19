using OpenTK.Mathematics;

namespace Hyper;

public static class SerializationHelper
{
    public static string Serialize(System.Numerics.Vector3 vector)
    {
        return $"{vector.X}_{vector.Y}_{vector.Z}";
    }
    
    public static string Serialize(Vector3 vector)
    {
        return $"{vector.X}_{vector.Y}_{vector.Z}";
    }
    
    public static Vector3 Deserialize(string vector)
    {
        var split = vector.Split('_');
        return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
    }
}