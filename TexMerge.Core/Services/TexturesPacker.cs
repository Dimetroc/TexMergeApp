using ImageMagick;
using TexMerge.Core.Models;

namespace TexMerge.Core.Services
{
    internal class TexturesPacker
    {
        private CombineOptions _data;
        private Action<string> _addLineToConsole;

        public TexturesPacker(CombineOptions data, Action<string> lineToConsole)
        {
            _data = data;
            _addLineToConsole = lineToConsole;
        }

        public void PackTextures()
        {

            if (string.IsNullOrEmpty(_data.RoughnessMap) || !System.IO.File.Exists(_data.RoughnessMap))
            {
                _addLineToConsole($"- Roughness map mising can't combine maps! {_data.RoughnessMap}");
                return;
            }

            if (string.IsNullOrEmpty(_data.MetalnessMap) || !System.IO.File.Exists(_data.MetalnessMap))
            {
                _addLineToConsole($"- Metalness map mising can't combine maps! {_data.MetalnessMap}");
                return;
            }

            if (string.IsNullOrEmpty(_data.AoMap) || !System.IO.File.Exists(_data.AoMap))
            {
                _addLineToConsole($"- Ambient occlusion map mising can't combine maps! {_data.AoMap}");
                return;
            }

            if (string.IsNullOrEmpty(_data.Name))
            {
                var fileName = Path.GetFileNameWithoutExtension(_data.RoughnessMap);
                foreach (var end in Constants.Roughness)
                {
                    if (fileName.EndsWith(end))
                    {
                        fileName = fileName.Substring(0, fileName.Length - end.Length);
                    }
                }
                _data.PackName = fileName;
            }
            else
            {
                _data.PackName = _data.Name;
            }

            using (var redImage = new MagickImage(_data.AoMap))
            {
                using (var greenImage = new MagickImage(_data.RoughnessMap))
                {
                    using (var blueImage = new MagickImage(_data.MetalnessMap))
                    {


                        if (redImage.Width == greenImage.Width && redImage.Height == greenImage.Height &&
                            redImage.Width == blueImage.Width && redImage.Height == blueImage.Height)
                        {

                            using (MagickImageCollection images = new MagickImageCollection())
                            {
                                images.Add(redImage);
                                images.Add(greenImage);
                                images.Add(blueImage);
                                using (MagickImage image = new MagickImage(images.Combine()))
                                {
                                    var path = _data.OutputPath + "/" + _data.PackName + Constants.AoRM + ".png";
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }
                                    image.Write(path);

                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
