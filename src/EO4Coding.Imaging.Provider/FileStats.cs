using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EO4Coding.Imaging
{

    /// <summary>
    /// Use to maintain clean dir structure
    /// </summary>
    public class FileStats
    {
        IImageFactory _imageFactory;


        internal FileStats(IImageFactory factory, IEnumerable<string> dbRefs)
        {
            AllDbImages = dbRefs;
            _imageFactory = factory;
            Analyze();
        }
        internal void Analyze()
        {
            DirectoryInfo di = new DirectoryInfo(_imageFactory.ImageDirectory);
            AllFileImages = di.GetFiles();
            HashSet<string> fi = new HashSet<string>(AllFileImages.Select(x => x.Name));

            NoDbRefs = AllFileImages.Where(f => !fi.Contains(f.Name)).ToArray();
            NoFileRefs = AllDbImages.Except(fi);

        }

        public FileInfo[] AllFileImages { get; internal set; }
        public IEnumerable<string> AllDbImages { get; internal set; }
        public FileInfo[] NoDbRefs { get; internal set; }
        public IEnumerable<string> NoFileRefs { get; set; }

    }
}
