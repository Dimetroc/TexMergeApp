namespace TexMerge.Core.Models
{
    public class CombineOptions
    {
        public bool UseExtra = false;
        public bool PackExtra = false;
        public bool ReplaceTransperent = true;

        public string Name = "Result";
        public string PackName = string.Empty;

        public string RoughnessMap = string.Empty;
        public string MetalnessMap = string.Empty;
        public string AoMap = string.Empty;

        public string InputPath = string.Empty;
        public string OutputPath = string.Empty;

        public CancellationTokenSource CancellationTokenSource;
    }
}