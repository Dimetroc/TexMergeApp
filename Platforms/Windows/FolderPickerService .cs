using TexMergeApp.Services;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace TexMergeApp.Platforms.Windows
{
    public class FolderPickerService : IFolderPicker
    {
        public async Task<string> PickFolderAsync()
        {
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");                           // allow all folders

            // grab the WinUI Window handle
            var window = Application.Current.Windows[0].Handler.PlatformView as Microsoft.UI.Xaml.Window;
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(picker, hwnd);

            var folder = await picker.PickSingleFolderAsync();
            return folder?.Path;
        }
    }
}
