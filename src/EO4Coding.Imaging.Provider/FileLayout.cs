using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EO4Coding.Imaging
{

    /// <summary>
    /// This class will format a filepath to include the size that is cached from the original, it handles all the details to get a full name from the orignal that can be used for caching
    /// Example image/Name.jpg(Name)  -> image/cache/Name~small.jpg(FullName)
    /// </summary>
    public class FileLayout
    {
        const char _delimeter = '~';

        IImageFactory _imageFactory;

        public FileLayout(IImageFactory imageFactory,string name,Size size)
        {
            Init(imageFactory, name, size);
        }
        public FileLayout(IImageFactory imageFactory, string name, string size)
        {
            Init(imageFactory, name, imageFactory.GetSize(size));
        }
        public FileLayout(IImageFactory imageFactory, string name)
        {
            Init(imageFactory, name, null);
        }

        /// <summary>
        /// Construct the full name
        /// </summary>
        /// <param name="imageFactory">factory to use for construction,will use its sizes,Path and CachePath</param>
        /// <param name="name">the file name, this can be the Original name or a name that includes the size specification</param>
        /// <param name="size"> Size the the fullname will represent</param>
        void Init(IImageFactory imageFactory,string name,Size size )
        {
            _imageFactory = imageFactory;
            SetNames(name, size);
        }

        public string FilePath { get; private set; }
        /// <summary>
        /// File Extension
        /// </summary>
        public string Extension { get; private set;}
        /// <summary>
        /// File Name part of FullName
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Name of the original file expluding path
        /// </summary>
        public string OriginalName { get; private set; }

        /// <summary>
        /// Size that the FullFile include
        /// </summary>
        public Size Size { get; private set;  }

        /// <summary>
        /// Name Including size predicate and cached path if applicable
        /// </summary>
        public string FullName { get; private set; }

        string[] SplitSize(string name)
        {
            name = name.Trim(_delimeter);
            int index = name.LastIndexOf(_delimeter);
            string fl;
            string sz;
            if(index>0)
            {
                fl = name.Substring(0, index);
                sz = name.Substring(index + 1);
            }
            else
            {
                fl = name;
                sz = null;
            }
            return new string[] { fl, sz };
        }

        void SetNames(string name,Size size)
        {

            FilePath =name.UrlDirectory();
            string nm = Path.GetFileNameWithoutExtension(name);
            if (string.IsNullOrEmpty(nm))
            {
                OriginalName = "";
                FullName = "";
                Name = "";
                return;
            }
            Extension = Path.GetExtension(name);
            if (Extension.Length <= 1) return;
            Extension = Extension.Substring(1);
            string[] fs = SplitSize(nm);   //nm.Split('~');
            OriginalName = $"{fs[0]}.{Extension}";
            if (size == null) //Name passed through already has a size and no explicit size was given
            {
                if (!string.IsNullOrEmpty(fs[1]))
                {
                    Size = GetSize(fs[1]);
                }
                else Size = _imageFactory.OriginalSize;
            }
            else Size = size;

            IsOriginal = Size == _imageFactory.OriginalSize;
            if (IsOriginal)
            {
                Name = OriginalName;
                FullName = $"{_imageFactory.ImageUrl}{Name}";
            }
            else
            {
                Name = $"{fs[0]}{_delimeter}{Size?.Name}.{Extension}";
                FullName = $"{_imageFactory.ImageUrl}{Name}";
            }

        }

        public bool IsOriginal { get; private set; }

        Size GetSize(string size)
        {
            return _imageFactory.GetSize(size);
        }

        public override string ToString()
        {
            return FullName;
        }

    }

    static class UrlExtensions
    {
        static public string UrlCombine(this string url1,string url2)
        {
            if (url1.Length == 0)
            {
                return url2;
            }

            if (url2.Length == 0)
            {
                return url1;
            }

            url1 = url1.TrimEnd('/', '\\');
            url2 = url2.TrimStart('/', '\\');

            return string.Format("{0}/{1}", url1, url2);
        }

        static public string UrlDirectory(this string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            return url.Substring(0,url.LastIndexOf('/')+1);
            
        }

    }


}
