namespace TexMerge.Core.Enums
{
    public enum CombinerResult
    {
        Success = 0,
        FailedResolution = 1,
        FailedMapsAmount = 2,
    }

    [Flags]
    public enum CombineFlags
    {
        None = 0,
        UseExtra = 1 << 0,
        PackExtra = 1 << 1,
        ReplaceTransparent = 1 << 2,
        JpgSave = 1 << 3
    }

    [Flags]
    public enum BaseMaps
    {
        None = 0,
        Roughness = 1 << 0,
        Metallic = 1 << 1,
        AmbientOcclusion = 1 << 2,
        NormalDX = 1 << 3,
        NormalOGL = 1 << 4,
        Normal = 1 << 5,

        All = Roughness | Metallic | AmbientOcclusion | NormalDX | NormalOGL | Normal
    }

    [Flags]
    public enum ExtraMaps
    {
        None = 0,
        Height = 1 << 0,
        Emissive = 1 << 1,
        Diffuse = 1 << 2,
        Specular = 1 << 3,
        Glossiness = 1 << 4,
        Displacement = 1 << 5,
        Refraction = 1 << 6,
        Reflection = 1 << 7,

        All = Height | Emissive | Diffuse | Specular | Glossiness | Displacement | Refraction | Reflection
    }
}