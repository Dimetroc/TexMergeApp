using System.Text;
using TexMerge.Core.Models;
using TexMerge.Core.Services;

namespace TexMerge.Core
{
    public class TextureMerger : ITextureMerger
    {
        public CombineOptions Options
        {
            get => _data;
            set => _data = value;
        }

        private CombineOptions _data = new();
        private Action<string> _addLineToConsole = _ => { }; // default no-op

        public TextureMerger() { }

        public TextureMerger(Action<string> consoleOutput)
        {
            _addLineToConsole = consoleOutput;
        }

        public void SetLogger(Action<string> log)
        {
            _addLineToConsole = log ?? (_ => { });
        }

        public async Task CombineAsync()
        {
            await Task.Run(() =>
            {
                var token = _data.CancellationTokenSource?.Token ?? CancellationToken.None;

                _addLineToConsole("--------------------------------------------------------Start--------------------------------------------------------");

                if (token.IsCancellationRequested)
                {
                    _addLineToConsole("Operation cancelled before start.");
                    return;
                }

                var setsCombiner = new SetsCombiner(_data, _addLineToConsole, token);
                setsCombiner.CombineSets();

                if (token.IsCancellationRequested)
                {
                    _addLineToConsole("Operation cancelled after combine sets.");
                    return;
                }

                if (_data.PackExtra)
                {
                    var packer = new TexturesPacker(_data, _addLineToConsole, token);
                    packer.PackTextures();
                }

                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
            });
        }


        public async Task PackAsync()
        {
            await Task.Run(() =>
            {
                var token = _data.CancellationTokenSource?.Token ?? CancellationToken.None;

                _addLineToConsole("--------------------------------------------------------Start--------------------------------------------------------");

                if (token.IsCancellationRequested)
                {
                    _addLineToConsole("Operation cancelled before start.");
                    return;
                }

                var packer = new TexturesPacker(_data, _addLineToConsole, token);
                packer.PackTextures();

                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
            });
        }
    }
}
