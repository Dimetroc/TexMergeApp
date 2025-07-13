using ImageMagick;
using TexMerge.Core.Models;

namespace TexMerge.Core.Services
{
    internal class TexturesPacker
    {
        private readonly CombineOptions _data;
        private readonly Action<string> _addLineToConsole;
        private readonly CancellationToken _token;

        public TexturesPacker(CombineOptions data, Action<string> lineToConsole, CancellationToken token)
        {
            _data = data;
            _addLineToConsole = lineToConsole;
            _token = token;
        }

        public void PackTextures()
        {
            if (_token.IsCancellationRequested) return;

            if (string.IsNullOrEmpty(_data.RoughnessMap) || !File.Exists(_data.RoughnessMap))
            {
                _addLineToConsole("- Roughness map missing, can't combine maps! " + _data.RoughnessMap);
                return;
            }
            if (_token.IsCancellationRequested) return;

            if (string.IsNullOrEmpty(_data.MetalnessMap) || !File.Exists(_data.MetalnessMap))
            {
                _addLineToConsole("- Metalness map missing, can't combine maps! " + _data.MetalnessMap);
                return;
            }
            if (_token.IsCancellationRequested) return;

            if (string.IsNullOrEmpty(_data.AoMap) || !File.Exists(_data.AoMap))
            {
                _addLineToConsole("- Ambient occlusion map missing, can't combine maps! " + _data.AoMap);
                return;
            }

            if (_token.IsCancellationRequested) return;

            if (string.IsNullOrEmpty(_data.Name))
            {
                var fileName = Path.GetFileNameWithoutExtension(_data.RoughnessMap);
                foreach (var end in Constants.Roughness)
                {
                    if (fileName.EndsWith(end))
                    {
                        fileName = fileName[..^end.Length];
                        break;
                    }
                }
                _data.PackName = fileName;
            }
            else
            {
                _data.PackName = _data.Name;
            }

            using var redImage = new MagickImage(_data.AoMap);
            if (_token.IsCancellationRequested) return;

            using var greenImage = new MagickImage(_data.RoughnessMap);
            if (_token.IsCancellationRequested) return;

            using var blueImage = new MagickImage(_data.MetalnessMap);
            if (_token.IsCancellationRequested) return;

            if (redImage.Width == greenImage.Width && redImage.Height == greenImage.Height &&
                redImage.Width == blueImage.Width && redImage.Height == blueImage.Height)
            {
                using var images = new MagickImageCollection();
                images.Add(redImage);
                if (_token.IsCancellationRequested) return;

                images.Add(greenImage);
                if (_token.IsCancellationRequested) return;

                images.Add(blueImage);
                if (_token.IsCancellationRequested) return;

                using var image = new MagickImage(images.Combine());
                if (_token.IsCancellationRequested) return;

                var path = Path.Combine(_data.OutputPath, _data.PackName + Constants.AoRM + ".png");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                if (_token.IsCancellationRequested) return;

                image.Write(path);
            }
        }
    }
}
