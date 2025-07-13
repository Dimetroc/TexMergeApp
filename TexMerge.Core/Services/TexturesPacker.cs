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

            var aoMap = string.Empty;
            var roughnessMap = string.Empty;
            var metallicMap = string.Empty;

            roughnessMap = FindFileBySuffixes(_data.InputPath, Constants.Roughness, _data.JpgSave);
            if (string.IsNullOrEmpty(roughnessMap) || !File.Exists(roughnessMap))
            {
                _addLineToConsole("- Roughness map missing, can't combine maps! " + roughnessMap);
                return;
            }
            if (_token.IsCancellationRequested) return;

            metallicMap = FindFileBySuffixes(_data.InputPath, Constants.Metallic, _data.JpgSave);
            if (string.IsNullOrEmpty(metallicMap) || !File.Exists(metallicMap))
            {
                _addLineToConsole("- Metallic map missing, can't combine maps! " + metallicMap);
                return;
            }
            if (_token.IsCancellationRequested) return;

            aoMap = FindFileBySuffixes(_data.InputPath, Constants.Ao, _data.JpgSave);
            if (string.IsNullOrEmpty(aoMap) || !File.Exists(aoMap))
            {
                _addLineToConsole("- Ambient occlusion map missing, can't combine maps! " + aoMap);
                return;
            }

            if (_token.IsCancellationRequested) return;

            
            using var redImageSource = new MagickImage(aoMap);
            var redImage = redImageSource.Separate(Channels.Red).First();
            if (_token.IsCancellationRequested) return;

            using var greenImageSource = new MagickImage(roughnessMap);
            var greenImage = greenImageSource.Separate(Channels.Red).First();
            if (_token.IsCancellationRequested) return;

            using var blueImageSource = new MagickImage(metallicMap);
            var blueImage = blueImageSource.Separate(Channels.Red).First();
            if (_token.IsCancellationRequested) return;

            if (redImage.Width == greenImage.Width && redImage.Height == greenImage.Height &&
                redImage.Width == blueImage.Width && redImage.Height == blueImage.Height)
            {
                using var images = new MagickImageCollection { redImage, greenImage, blueImage };


   
                if (_token.IsCancellationRequested) return;

                using var image = new MagickImage(images.Combine());
                if (_token.IsCancellationRequested) return;

                var path = GetOutputPath(roughnessMap);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                if (_token.IsCancellationRequested) return;

                if (_data.JpgSave)
                {
                    image.Format = MagickFormat.Jpeg;
                    image.Alpha(AlphaOption.Remove);
                    image.Quality = 90;
                }

                image.Write(path);

                _addLineToConsole($"+ {GetName(roughnessMap)}{Constants.AoRM}{(_data.JpgSave ? ".jpg" : ".png")} map is packed!");
            }
            else
            {
                _addLineToConsole("- Sizes do not match, can't pack maps! ");
            }
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
