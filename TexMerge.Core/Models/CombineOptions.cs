using TexMerge.Core.Enums;

namespace TexMerge.Core.Models
{
    public class CombineOptions
    {
        public bool UseExtra => CombineFlags.HasFlag(CombineFlags.UseExtra);
        public bool PackExtra => CombineFlags.HasFlag(CombineFlags.PackExtra);
        public bool ReplaceTransperent => CombineFlags.HasFlag(CombineFlags.ReplaceTransparent) || JpgSave;
        public bool JpgSave => CombineFlags.HasFlag(CombineFlags.JpgSave);

        public CombineFlags CombineFlags = CombineFlags.None;
        public BaseMaps BaseMaps = BaseMaps.All;
        public ExtraMaps ExtraMaps = ExtraMaps.All;

        public string Name = "Result";

        public string InputPath = string.Empty;
        public string OutputPath = string.Empty;

        public CancellationTokenSource CancellationTokenSource;
    }
}