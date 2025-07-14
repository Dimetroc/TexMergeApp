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

            if (!Directory.Exists(_data.OutputPath))
            {
                _addLineToConsole("Pack folder does not exists!");
                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
                return;
            }

            var aoMap = FindFileBySuffixes(_data.OutputPath, Constants.Ao, _data.JpgSave);
            if (_token.IsCancellationRequested) return;

            var roughnessMap = FindFileBySuffixes(_data.OutputPath, Constants.Roughness, _data.JpgSave);
            if (_token.IsCancellationRequested) return;

            var metallicMap = FindFileBySuffixes(_data.OutputPath, Constants.Metallic, _data.JpgSave);
            if (_token.IsCancellationRequested) return;

            if (string.IsNullOrEmpty(roughnessMap) || !File.Exists(roughnessMap))
            {
                _addLineToConsole("- Roughness map missing, can't combine maps! " + roughnessMap);
                return;
            }
            if (_token.IsCancellationRequested) return;

            if (string.IsNullOrEmpty(metallicMap) || !File.Exists(metallicMap))
            {
                _addLineToConsole("- Metallic map missing, can't combine maps! " + metallicMap);
                return;
            }
            if (_token.IsCancellationRequested) return;

            if (string.IsNullOrEmpty(aoMap) || !File.Exists(aoMap))
            {
                _addLineToConsole("- Ambient occlusion map missing, can't combine maps! " + aoMap);
                return;
            }
            if (_token.IsCancellationRequested) return;

            using var redImageSource = new MagickImage(aoMap);
            using var greenImageSource = new MagickImage(roughnessMap);
            using var blueImageSource = new MagickImage(metallicMap);
            if (_token.IsCancellationRequested) return;

            redImageSource.Alpha(AlphaOption.Remove);
            greenImageSource.Alpha(AlphaOption.Remove);
            blueImageSource.Alpha(AlphaOption.Remove);
            if (_token.IsCancellationRequested) return;

            var width = redImageSource.Width;
            var height = redImageSource.Height;

            if (greenImageSource.Width != width || greenImageSource.Height != height ||
                blueImageSource.Width != width || blueImageSource.Height != height)
            {
                _addLineToConsole("- Sizes do not match, can't pack maps! ");
                return;
            }

            var redChannel = redImageSource.Separate(Channels.Red).First();
            if (_token.IsCancellationRequested) return;
            var greenChannel = greenImageSource.Separate(Channels.Red).First();
            if (_token.IsCancellationRequested) return;
            var blueChannel = blueImageSource.Separate(Channels.Red).First();
            if (_token.IsCancellationRequested) return;

            using var resultImage = new MagickImage(MagickColors.Black, width, height);
            resultImage.Composite(redChannel, CompositeOperator.CopyRed);
            if (_token.IsCancellationRequested) return;
            resultImage.Composite(greenChannel, CompositeOperator.CopyGreen);
            if (_token.IsCancellationRequested) return;
            resultImage.Composite(blueChannel, CompositeOperator.CopyBlue);
            if (_token.IsCancellationRequested) return;

            var path = GetOutputPath(roughnessMap);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (_token.IsCancellationRequested) return;

            if (_data.JpgSave)
            {
                resultImage.Format = MagickFormat.Jpeg;
                resultImage.Quality = 90;
                resultImage.Alpha(AlphaOption.Remove);
            }

            resultImage.Write(path);
            _addLineToConsole($"+ {GetName(roughnessMap)}{Constants.AoRM}{(_data.JpgSave ? ".jpg" : ".png")} map is packed!");
        }

        private string GetOutputPath(string roughnessMap)
        {
            var fileName = GetName(roughnessMap);
            var extension = _data.JpgSave ? ".jpg" : ".png";
            return Path.Combine(_data.OutputPath, fileName + Constants.AoRM + extension);
        }

        private string GetName(string roughnessMap)
        {
            if (string.IsNullOrEmpty(_data.Name))
            {
                var fileName = Path.GetFileNameWithoutExtension(roughnessMap);
                foreach (var end in Constants.Roughness)
                {
                    if (fileName.EndsWith(end))
                    {
                        fileName = fileName[..^end.Length];
                        break;
                    }
                }
                return fileName;
            }
            else
            {
                return _data.Name;
            }
        }

        private string? FindFileBySuffixes(string folderPath, string[] suffixes, bool jpgSave)
        {
            if (!Directory.Exists(folderPath) || suffixes == null || suffixes.Length == 0)
                return null;

            var extension = jpgSave ? ".jpg" : ".png";
            var files = Directory.GetFiles(folderPath, "*" + extension, SearchOption.TopDirectoryOnly);

            var result = files.FirstOrDefault(file =>
                suffixes.Any(suffix => Path.GetFileNameWithoutExtension(file).EndsWith(suffix, StringComparison.OrdinalIgnoreCase)));

            if (!string.IsNullOrEmpty(result)) return result;

            extension = jpgSave ? ".png" : ".jpg";
            files = Directory.GetFiles(folderPath, "*" + extension, SearchOption.TopDirectoryOnly);

            return files.FirstOrDefault(file =>
                suffixes.Any(suffix => Path.GetFileNameWithoutExtension(file).EndsWith(suffix, StringComparison.OrdinalIgnoreCase)));
        }
    }
}