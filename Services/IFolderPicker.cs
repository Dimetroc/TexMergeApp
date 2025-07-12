using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
