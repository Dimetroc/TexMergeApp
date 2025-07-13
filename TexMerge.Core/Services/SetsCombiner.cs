using ImageMagick;
using TexMerge.Core.Models;
using TexMerge.Core.Enums;

namespace TexMerge.Core.Services
{
    internal class SetsCombiner
    {
        private readonly CombineOptions _data;
        private readonly Action<string> _addLineToConsole;
        private readonly CancellationToken _token;

        public SetsCombiner(CombineOptions data, Action<string> addLineToConsole, CancellationToken token)
        {
            _data = data;
            _addLineToConsole = addLineToConsole;
            _token = token;
        }

        public void CombineSets()
        {
            if (_token.IsCancellationRequested) return;
            if (!Directory.Exists(_data.InputPath))
            {
                _addLineToConsole("Source folder does not exists!");
                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
                return;
            }

            if (_token.IsCancellationRequested) return;
            if (!Directory.Exists(_data.OutputPath))
            {
                _addLineToConsole("Destination folder does not exists!");
                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
                return;
            }

            var finder = new SetFinder(_data.InputPath);
            if (_token.IsCancellationRequested) return;

            if (!finder.HasFiles)
            {
                _addLineToConsole("No .png textures found in the source folder!");
                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
                return;
            }

            if (_token.IsCancellationRequested) return;

            if (!TryGetCombiner("Base color", Constants.Color, finder, out var combiner))
            {
                _addLineToConsole("Color textures were not found in the source folder!");
                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
                return;
            }

            if (_token.IsCancellationRequested) return;

            if (_data.BaseMaps.HasFlag(BaseMaps.Roughness))
                _data.RoughnessMap = WriteSet("Roughness", Constants.Roughness, Constants.RoughnessColor, finder, combiner);
            if (_token.IsCancellationRequested) return;
            if (_data.BaseMaps.HasFlag(BaseMaps.Metallic))
                _data.MetalnessMap = WriteSet("Metallic", Constants.Metallic, Constants.MetalliColor, finder, combiner);
            if (_token.IsCancellationRequested) return;
            if (_data.BaseMaps.HasFlag(BaseMaps.AmbientOcclusion))
                _data.AoMap = WriteSet("Ambient Occlusion", Constants.Ao, Constants.AoColor, finder, combiner);
            if (_token.IsCancellationRequested) return;

            if (_data.BaseMaps.HasFlag(BaseMaps.NormalDX))
                WriteSet("Normal DirectX", Constants.NormalDX, Constants.NormalDXColor, finder, combiner);
            if (_token.IsCancellationRequested) return;
            if (_data.BaseMaps.HasFlag(BaseMaps.NormalOGL))
                WriteSet("Normal OpenGl", Constants.NormalOGL, Constants.NormalOGLColor, finder, combiner);
            if (_token.IsCancellationRequested) return;
            if (_data.BaseMaps.HasFlag(BaseMaps.Normal))
                WriteSet("Normal", Constants.Normal, Constants.NormalColor, finder, combiner);
            if (_token.IsCancellationRequested) return;

            if (_data.UseExtra)
            {
                if (_data.ExtraMaps.HasFlag(ExtraMaps.Height))
                    WriteSet("Height", Constants.Height, Constants.HeightColor, finder, combiner);
                if (_token.IsCancellationRequested) return;
                if (_data.ExtraMaps.HasFlag(ExtraMaps.Emissive))
                    WriteSet("Emissive", Constants.Emissive, Constants.EmissiveColor, finder, combiner);
                if (_token.IsCancellationRequested) return;
                if (_data.ExtraMaps.HasFlag(ExtraMaps.Diffuse))
                    WriteSet("Diffuse", Constants.Diffuse, Constants.DiffusColor, finder, combiner);
                if (_token.IsCancellationRequested) return;
                if (_data.ExtraMaps.HasFlag(ExtraMaps.Specular))
                    WriteSet("Specular", Constants.Specular, Constants.SpecularColor, finder, combiner);
                if (_token.IsCancellationRequested) return;
                if (_data.ExtraMaps.HasFlag(ExtraMaps.Glossiness))
                    WriteSet("Glossiness", Constants.Glossiness, Constants.GlossinesColor, finder, combiner);
                if (_token.IsCancellationRequested) return;
                if (_data.ExtraMaps.HasFlag(ExtraMaps.Displacement))
                    WriteSet("Displacement", Constants.Displacement, Constants.DisplacementColor, finder, combiner);
                if (_token.IsCancellationRequested) return;
                if (_data.ExtraMaps.HasFlag(ExtraMaps.Refraction))
                    WriteSet("Index Of Refraction", Constants.Refraction, Constants.RefractionColor, finder, combiner);
                if (_token.IsCancellationRequested) return;
                if (_data.ExtraMaps.HasFlag(ExtraMaps.Reflection))
                    WriteSet("Reflection", Constants.Reflection, Constants.ReflectionColor, finder, combiner);
                if (_token.IsCancellationRequested) return;
            }

            var path = GetPath(combiner.Suffix);
            combiner.WriteColorMap(path);
            _addLineToConsole("+ Color map added!");
            combiner.Dispose();
            if (_data.ReplaceTransperent)
            {
                ReplaceTransperentPixels(path, Constants.ColorColor);
            }
        }

        private bool TryGetCombiner(string name, string[] suffixes, SetFinder finder, out Combiner combiner)
        {
            combiner = null;
            var found = suffixes.Any(finder.HasSpecificFiles);
            if (!found)
            {
                _addLineToConsole("- " + name + " images not found!");
                return false;
            }

            combiner = new Combiner(finder.Files, finder.Suffix);
            return true;
        }

        private string WriteSet(string setName, string[] suffixes, MagickColor replacementColor, SetFinder finder, Combiner combiner)
        {
            if (FindInSet(suffixes, finder))
            {
                var path = GetPath(finder.Suffix);
                var result = combiner.WriteSet(path, finder.Files);

                if (result == Enums.CombinerResult.Success)
                {
                    if (_data.ReplaceTransperent)
                    {
                        ReplaceTransperentPixels(path, replacementColor);
                    }
                    _addLineToConsole("+ " + setName + " map added!");
                    return path;
                }

                _addLineToConsole("- " + setName + GetErrorText(finder.Suffix, result));
            }
            else
            {
                _addLineToConsole("- " + setName + " not found.");
            }
            return string.Empty;
        }

        private bool FindInSet(string[] suffixes, SetFinder finder)
        {
            return suffixes.Any(finder.HasSpecificFiles);
        }

        private string GetPath(string suffix)
        {
            return _data.OutputPath + "/" + _data.Name + suffix + ".png";
        }

        private void ReplaceTransperentPixels(string path, MagickColor replacementColor)
        {
            using var image = new MagickImage(path);
            image.ColorAlpha(replacementColor);
            image.Write(path);
        }

        private string GetErrorText(string suffix, Enums.CombinerResult result)
        {
            return result switch
            {
                Enums.CombinerResult.FailedMapsAmount => suffix + " maps amount do not match color maps amount!",
                Enums.CombinerResult.FailedResolution => suffix + " maps resolution and/or depth mismatch!",
                _ => suffix + " maps merged successfully!",
            };
        }
    }
}
