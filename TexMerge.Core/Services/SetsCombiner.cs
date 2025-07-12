using ImageMagick;
using TexMerge.Core.Models;

namespace TexMerge.Core.Services
{
    internal class SetsCombiner
    {
        private CombineOptions _data;
        private Action<string> _addLineToConsole;

        public SetsCombiner(CombineOptions data, Action<string> addLineToConsole)
        {
            _data = data;
            _addLineToConsole = addLineToConsole;
        }

        public void CombineSets()
        {
            if (!System.IO.Directory.Exists(_data.InputPath))
            {
                _addLineToConsole("Source folder does not exists!");
                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
                return;
            }

            if (!System.IO.Directory.Exists(_data.OutputPath))
            {
                _addLineToConsole("Destination folder does not exists!");
                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
                return;
            }

            var finder = new SetFinder(_data.InputPath);

            if (!finder.HasFiles)
            {
                _addLineToConsole("No .png textures found in the source folder!");
                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
                return;
            }

            if (!TryGetCombiner("Base color", Constants.Color, finder, out var combiner))
            {
                _addLineToConsole("Color textures were not found in the source folder!");
                _addLineToConsole("-------------------------------------------------------Finish-------------------------------------------------------");
                return;
            }


            _data.RoughnessMap = WriteSet("Roughness", Constants.Roughness, Constants.RoughnessColor, finder, combiner);
            _data.MetalnessMap = WriteSet("Metallic", Constants.Metallic, Constants.MetalliColor, finder, combiner);
            _data.AoMap = WriteSet("Ambient Occlusion", Constants.Ao, Constants.AoColor, finder, combiner);
            WriteSet("Normal DirectX", Constants.NormalDX, Constants.NormalDXColor, finder, combiner);
            WriteSet("Normal OpenGl", Constants.NormalOGL, Constants.NormalOGLColor, finder, combiner);
            WriteSet("Normal", Constants.Normal, Constants.NormalColor, finder, combiner);

            if (_data.UseExtra)
            {
                WriteSet("Height", Constants.Height, Constants.HeightColor, finder, combiner);
                WriteSet("Emissive", Constants.Emissive, Constants.EmissiveColor, finder, combiner);
                WriteSet("Diffuse", Constants.Diffuse, Constants.DiffusColor, finder, combiner);
                WriteSet("Specular", Constants.Specular, Constants.SpecularColor, finder, combiner);
                WriteSet("Glossiness", Constants.Glossiness, Constants.GlossinesColor, finder, combiner);
                WriteSet("Displacement", Constants.Displacement, Constants.DisplacementColor, finder, combiner);
                WriteSet("Index Of Refraction", Constants.Refraction, Constants.RefractionColor, finder, combiner);
                WriteSet("Reflection", Constants.Reflection, Constants.ReflectionColor, finder, combiner);
            }

            var path = GetPath(combiner.Suffix);
            combiner.WriteColorMap(GetPath(combiner.Suffix));
            combiner.Dispose();
            if (_data.ReplaceTransperent)
            {
                ReplaceTransperentPixels(path, Constants.ColorColor);
            }
        }

        private bool TryGetCombiner(string name, string[] suffixes, SetFinder finder, out Combiner combiner)
        {
            combiner = null;
            var found = false;
            for (int i = 0; i < suffixes.Length; i++)
            {
                if (!finder.HasSpecificFiles(suffixes[i])) continue;
                found = true;
                break;
            }

            if (!found)
            {
                _addLineToConsole("- " + name + " images not found!");
                return false;
            }


            if (!finder.HasFiles)
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
            for (int i = 0; i < suffixes.Length; i++)
            {
                if (finder.HasSpecificFiles(suffixes[i])) return true;
            }
            return false;
        }

        private string GetPath(string suffix)
        {
            return _data.OutputPath + "/" + _data.Name + suffix + ".png";
        }

        private void ReplaceTransperentPixels(string path, MagickColor replacementColor)
        {
            using (var image = new MagickImage(path))
            {
                image.ColorAlpha(replacementColor);
                image.Write(path);
            }
        }

        private string GetErrorText(string suffix, Enums.CombinerResult result)
        {
            switch (result)
            {
                case Enums.CombinerResult.FailedMapsAmount:
                    return suffix + " maps amount do not match color maps amount!";
                case Enums.CombinerResult.FailedResolution:
                    return suffix + " maps resolution and/or depth mismatch!";
            }
            return suffix + "maps merged succesfuly!";
        }
    }
}
