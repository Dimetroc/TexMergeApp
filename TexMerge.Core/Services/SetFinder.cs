
namespace TexMerge.Core.Services
{
    internal class SetFinder
    {
        public string Suffix { get; private set; }

        private const string _extension = "*.png";
        private const string _extensionCheck = "png";
        private const string _extensionCheckUpper = "PNG";
        public string[] Files { get; private set; }
        private string[] _filesInFolder;
        private string _path;

        public bool HasFiles => _filesInFolder != null && _filesInFolder.Length > 0;

        public SetFinder(string path)
        {
            _path = path;
            Files = Array.Empty<string>();
            _filesInFolder = Array.Empty<string>();
            Suffix = string.Empty;

            SetFilesInFolder();
        }

        private void SetFilesInFolder()
        {
            if (string.IsNullOrEmpty(_path)) return;
            if (!Directory.Exists(_path)) return;
            _filesInFolder = Directory.GetFiles(_path, _extension);
        }

        public bool HasSpecificFiles(string suffix)
        {
            if (string.IsNullOrEmpty(suffix)) return false;
            Suffix = suffix;
            FilterFilesBySuffix();
            return Files != null && Files.Length > 0;
        }


        private void FilterFilesBySuffix()
        {
            var result = new List<string>();
            var end = Suffix;
            for (int i = 0; i < _filesInFolder.Length; i++)
            {
                var fileName = _filesInFolder[i];
                if (!File.Exists(fileName)) continue;
                var split = fileName.Split('.');
                if (split.Length < 2) continue;
                var name = split[^2];
                var extension = split[^1];
                if (!extension.EndsWith(_extensionCheck) && !name.EndsWith(_extensionCheckUpper)) continue;
                if (!name.EndsWith(end, StringComparison.OrdinalIgnoreCase)) continue;
                result.Add(fileName);
            }

            Files = result.ToArray();
        }
    }
}
