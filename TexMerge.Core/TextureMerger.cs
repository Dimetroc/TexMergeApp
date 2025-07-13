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
            PrintBaseOptions();
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

        private void PrintBaseOptions()
        {
            _addLineToConsole("----------------------------------------------------Base maps:---------------------------------------------------");
            PrintOptionSet("Base color", Constants.Color);
            PrintOptionSet("Roughness", Constants.Roughness);
            PrintOptionSet("Metallic", Constants.Metallic);
            PrintOptionSet("Ambient Occlusion", Constants.Ao);
            PrintOptionSet("Normal DirectX", Constants.NormalDX);
            PrintOptionSet("Normal OpenGL", Constants.NormalOGL);
            PrintOptionSet("Normal", Constants.Normal);
            _addLineToConsole("");
            _addLineToConsole("---------------------------------------------------Extra maps:---------------------------------------------------");
            PrintOptionSet("Height", Constants.Height);
            PrintOptionSet("Emissive", Constants.Emissive);
            PrintOptionSet("Diffuse", Constants.Diffuse);
            PrintOptionSet("Specular", Constants.Specular);
            PrintOptionSet("Glossiness", Constants.Glossiness);
            PrintOptionSet("Displacement", Constants.Displacement);
            PrintOptionSet("Index Of Refraction", Constants.Refraction);
            PrintOptionSet("Reflection", Constants.Reflection);
            _addLineToConsole("----------------------------------------------------------------------------------------------------------------------");
            _addLineToConsole("");

        }

        private void PrintOptionSet(string name, string[] options)
        {
            if (string.IsNullOrEmpty(name)) { return; }
            if (options == null || options.Length == 0) { return; }

            var str = new StringBuilder();
            str.Append(name);
            str.Append(": ");
            foreach (var option in options)
            {
                str.Append(option);
                str.Append(",");
            }
            _addLineToConsole(str.ToString());
            str.Clear();
        }
    }
}
