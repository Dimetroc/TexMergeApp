namespace TexMergeApp.Services
{
    internal interface IFolderPicker
    {
        /// <summary>
        /// Pops up a native folder‐selection dialog and returns the selected path (or null).
        /// </summary>
        Task<string> PickFolderAsync();
    }
}
