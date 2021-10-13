using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCopyLibrary
{
    public class CopyFiles : ICopyFiles
    {

        /// <summary>
        /// Copy file from source to destination
        /// </summary>
        /// <param name="sourceFileInfo">Source file path</param>
        /// <param name="destinationFileInfo">Destination file path</param>
        /// <param name="progressCallback">Callback function to update the progress</param>
        private void Copyfile(FileInfo sourceFileInfo, FileInfo destinationFileInfo, Action<string, int> progressCallback)
        {

            const int bufferSize =  1024 * 1024; // taking a large buffer size, its up to us if we want to increase or decrease it
            byte[] buffer = new byte[bufferSize];
                 
            int progress = 0, lastProgress = 0, readCount = 0;
            long len = sourceFileInfo.Length;
            float fileLength = len;
            Task writer = null; 
            using (var sourceFile = sourceFileInfo.OpenRead())
            using (var destinationFile = destinationFileInfo.OpenWrite())
            {
                destinationFile.SetLength(sourceFile.Length);// matching source and destination file length
                for (long progressSize = 0; progressSize < len; progressSize += readCount)
                {
                    // Making sure that we have made some progress before we can call the callback function
                    if ((progress = ((int)((progressSize / fileLength) * 100))) != lastProgress)
                    {
                        progressCallback($"Copying {sourceFileInfo.Name} from {Path.GetDirectoryName(sourceFileInfo.FullName) } to {Path.GetDirectoryName(destinationFileInfo.FullName)}", lastProgress = progress);
                        // Calling the Callback function to update the progress
                    }
                    readCount = sourceFile.Read( buffer, 0, bufferSize);
                    writer?.Wait(); //If writer is in progress for the last buffer then wait
                    writer = destinationFile.WriteAsync(buffer , 0, readCount);// creating task to write the file asyncronously                    
                }
                writer?.Wait();
            }
        }

        public void CopyFolder(string sourceFolder, string destFolder, Action<string, int> progressCallback)
        {
            try
            {
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);
                string[] files = Directory.GetFiles(sourceFolder);
                //Loop for all the files in a directory
                foreach (string file in files)
                {
                    var _source = new FileInfo(file);
                    string name = Path.GetFileName(Path.Combine(destFolder, file));
                    string dest = Path.Combine(destFolder, name);
                    var _destination = new FileInfo(dest);
                    if (File.Exists(dest)) // If file already exists then deleting, we can do what we want
                        File.Delete(dest);

                    Copyfile(_source, _destination, (fileName, progressPercentage) => progressCallback(fileName, progressPercentage));
                }
                string[] folders = Directory.GetDirectories(sourceFolder);
                //loop for sub folders
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    //recursive calls for each subfolder
                    CopyFolder(folder, dest, progressCallback);
                }
            }
            catch (Exception ex)
            {
                //we can also log these error before throwing it to client
                throw ex;
            }
        }

    }
}
