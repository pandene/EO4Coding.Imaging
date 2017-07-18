using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EO4Coding.Imaging
{
    public interface IImageProvider
    {
        /// <summary>
        /// FileName as presented on the physical drive
        /// </summary>
        /// <param name="fileName">urlFilePath</param>
        /// <param name="size">Size param as loaded in factory</param>
        /// <returns>the physucal path to file</returns>
        string GetActualFileName(string fileName, string size,bool isOriginal=false);
        string GetActualFileName(string fileName, Size size, bool isOriginal = false);
        /// <summary>
        /// FileName that will be requested from web, this will never show the cache dir
        /// </summary>
        /// <param name="fileName">urlFilePath</param>
        /// <param name="size">Size param as loaded in factory</param>
        /// <returns>The formated file including the size</returns>
        string GetFullUrlName(string fileName, Size size);
        string GetFullUrlName(string fileName, string size);
        /// <summary>
        /// Returns detail data about the file
        /// </summary>
        /// <param name="fileName">urlFilePath</param>
        /// <param name="size">Size param as loaded in factory</param>
        /// <returns>FileLayout</returns>
        FileLayout GetFileLayout(string fileName, Size size=null);
        /// <summary>
        /// Writes the contect of the file to a stream, such as response output stream
        /// </summary>
        /// <param name="fileLayout">File Detail</param>
        /// <param name="output">Output stream to write to</param>
        /// <returns>Async Task</returns>
        Task WriteFileAsync(FileLayout fileLayout,Stream output);

        void ClearCache();
        void CleanUp(IEnumerable<string> dbRefs);
        void CleanUp(FileStats stats);
        FileStats GetFileStats(IEnumerable<string> dbRefs);
        Size GetSize(string size);
        Size OriginalSize { get; }
        string ImageDirectory { get; }
        string CacheDirectory { get; }
        string ImageUrl { get; }

    }




}
