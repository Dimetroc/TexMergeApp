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
        ReplaceTransparent = 1 << 2
    }
}