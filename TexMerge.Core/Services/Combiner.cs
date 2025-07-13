using ImageMagick;
using TexMerge.Core.Enums;
using TexMerge.Core.Models;

namespace TexMerge.Core.Services
{
    internal class Combiner
    {
        public string Suffix { get; private set; }
        private string[] _baseColor;
        private uint _baseWidth;
        private uint _baseHeight;
        private uint _baseDepth;
        private readonly bool _jpgSave;

        public Combiner(string[] baseColor, string suffix, bool jpgSave)
        {
            Suffix = suffix;
            _baseColor = baseColor;
            _jpgSave = jpgSave;
            SetBaseData();
        }

        public CombinerResult WriteSet(string path, string[] files)
        {
            if (files == null || files.Length != _baseColor.Length)
            {
                return CombinerResult.FailedMapsAmount;
            }

            if (!CheckImagesSizes(files))
            {
                return CombinerResult.FailedResolution;
            }

            WriteMap(path, files);
            return CombinerResult.Success;
        }

        private void WriteMap(string path, string[] files)
        {
            var settings = new MagickReadSettings
            {
                Width = _baseWidth,
                Height = _baseHeight,
                BackgroundColor = MagickColors.Transparent
            };

            using (var resultImage = new MagickImage("xc:none", settings))
            {
                for (var i = 0; i < files.Length; i++)
                {
                    using (var baseMap = new MagickImage(_baseColor[i]))
                    using (var target = new MagickImage(files[i]))
                    {
                        target.Composite(baseMap, CompositeOperator.CopyAlpha);
                        resultImage.Composite(target, CompositeOperator.SrcOver);
                    }
                }

                if (_jpgSave)
                {
                    resultImage.Format = MagickFormat.Jpeg;
                    resultImage.Alpha(AlphaOption.Remove);
                    resultImage.Quality = 90;
                }

                resultImage.Write(path);
            }
        }

        public void WriteColorMap(string path)
        {
            if (_baseColor == null || _baseColor.Length == 0) return;

            var settings = new MagickReadSettings
            {
                Width = _baseWidth,
                Height = _baseHeight,
                BackgroundColor = MagickColors.Transparent
            };

            using (var resultImage = new MagickImage("xc:none", settings))
            {
                for (var i = 0; i < _baseColor.Length; i++)
                {
                    using (var baseMap = new MagickImage(_baseColor[i]))
                    {
                        resultImage.Composite(baseMap, CompositeOperator.SrcOver);
                    }
                }

                if (_jpgSave)
                {
                    resultImage.Format = MagickFormat.Jpeg;
                    resultImage.Alpha(AlphaOption.Remove);
                    resultImage.Quality = 90;
                }

                resultImage.Write(path);
            }
        }

        private bool CheckImagesSizes(string[] files)
        {
            foreach (var file in files)
            {
                var correctSize = true;
                using (var image = new MagickImage(file))
                {
                    correctSize = image.Width == _baseWidth && image.Height == _baseHeight && image.Depth == _baseDepth;
                }

                if (!correctSize) return false;
            }
            return true;
        }

        public void Dispose()
        {
            _baseColor = Array.Empty<string>();
        }

        private void SetBaseData()
        {
            using (var first = new MagickImage(_baseColor[0]))
            {
                _baseWidth = first.Width;
                _baseHeight = first.Height;
                _baseDepth = first.Depth;
            }
        }
    }
}
