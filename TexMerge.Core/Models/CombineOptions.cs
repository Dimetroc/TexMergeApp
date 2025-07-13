using TexMerge.Core.Enums;

namespace TexMerge.Core.Models
{
    public class CombineOptions
    {

        public bool UseExtra => CombineFlags.HasFlag(CombineFlags.UseExtra);
        public bool PackExtra => CombineFlags.HasFlag(CombineFlags.PackExtra);
        public bool ReplaceTransperent => CombineFlags.HasFlag(CombineFlags.ReplaceTransparent);

        public CombineFlags CombineFlags = CombineFlags.None;
        public BaseMaps BaseMaps = BaseMaps.All;
        public ExtraMaps ExtraMaps = ExtraMaps.All;

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