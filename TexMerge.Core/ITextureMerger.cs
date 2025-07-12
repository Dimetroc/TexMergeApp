using TexMerge.Core.Models;

namespace TexMerge.Core
{
    public interface ITextureMerger
    {
        CombineOptions Options { get; set; }

        void SetLogger(Action<string> log);

        Task CombineAsync();
        Task PackAsync();
    }
}
