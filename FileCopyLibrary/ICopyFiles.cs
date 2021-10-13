using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCopyLibrary
{
    public interface  ICopyFiles
    {
        /// <summary>
        /// Copies all the files and folders from Source firectory to destination
        /// </summary>
        /// <param name="sourceFolder">From Path</param>
        /// <param name="destFolder">To Path</param>
        /// <param name="progressCallback">Delegate to execute when progress changes</param>
        void CopyFolder(string sourceFolder, string destFolder, Action<string, int> progressCallback);
    }
}
