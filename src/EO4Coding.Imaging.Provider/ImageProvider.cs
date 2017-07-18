//using ImageProcessorCore;
using ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EO4Coding.Imaging
{
    public class ImageProvider:IImageProvider
    {
        string _imageDirectory;
        string _cacheDirectory;
        string _imageUrl;
        Dictionary<string, Size> _sizes = new Dictionary<string, Size>();

        public ImageProvider(string imageUrl,string imageDirectory ,string cacheDirectory=null,Size[] sizes=null)
        {
            if (string.IsNullOrEmpty(cacheDirectory)) cacheDirectory = Path.Combine(imageDirectory, @"cache\");
            _imageDirectory = !imageDirectory.EndsWith(@"\") && imageDirectory.Length>0?imageDirectory+@"\":imageDirectory;//?.TrimEnd('\\');
            _cacheDirectory = !cacheDirectory.EndsWith(@"\") && cacheDirectory.Length > 0 ? cacheDirectory + @"\" : cacheDirectory;//?.TrimEnd('\\');
            _imageUrl = !imageUrl.EndsWith("/") && imageUrl.Length > 0 ? imageUrl + @"/" : imageUrl;//?.TrimEnd('\\');
            if (!Directory.Exists(_cacheDirectory)) Directory.CreateDirectory(_cacheDirectory);
            InitSizes(sizes);
        }

        void InitSizes(Size[] sizes)
        {
            sizes=sizes ?? new Size[]
                {
                    new Size() {Name="original",MaxX=0,MaxY=0}, //use 0 not to change the original size, this is also registered under the original size and will return the image as is
                    new Size() {Name="original-cached",MaxX=0,MaxY=0}, //this return the original size, but it wil go through the resizing process which in most cases result in a smaller file size for the same image size, the compression algorithm used seem to compress more/beter, use the test cases to compare size. Test1.jpg in my example reduced from 14MB to 2.5MB using the same size
                    new Size() {Name="xlarge",MaxX=1900,MaxY=1200 },//Ex:name~xlarge.jpg
                    new Size() {Name="large",MaxX=1024,MaxY=768 },
                    new Size() {Name="medium",MaxX=800,MaxY=600 },
                    new Size() {Name="small",MaxX=300,MaxY=240 },
                    new Size() {Name="thumbnail",MaxX=160,MaxY=160 }


                };
            
            AddSizes(sizes);
            OriginalSize = GetSize("original");
        }
        public Size OriginalSize { get; set; }

        public string ImageDirectory
        {
            get
            {
                return _imageDirectory;
            }
        }

        public string CacheDirectory
        {
            get
            {
                return _cacheDirectory;
            }
        }

        public string ImageUrl { get { return _imageUrl; } }

        public string GetActualFileName(string fileName, string size, bool original = false)
        {
            return GetFileName(GetFileLayout(fileName, GetSize(size)), original);
        }

        public string GetActualFileName(string fileName, Size size,bool original =false)
        {
            return GetFileName(GetFileLayout(fileName,size),original);
        }
        public string GetFullUrlName(string fileName, string size)
        {
            return GetFullUrlName(fileName, GetSize(size));
        }

        public string GetFullUrlName(string fileName, Size size)
        {
            return GetFileLayout(fileName, size).FullName;
        }

        public FileLayout GetFileLayout(string fileName,Size size=null)
        {
            return new FileLayout(this, fileName, size);
        }

        public async Task WriteFileAsync(FileLayout fileLayout, Stream output)
        {
            string file = await EnsureFileAsync(fileLayout);
            if (String.IsNullOrEmpty(file)) return;
            using (Stream fl = File.OpenRead(file))
            {
                await fl.CopyToAsync(output);
            }


        }


        /// <summary>
        /// Make sure the fie in question exists. If it is a cached file it will create it from the original if it doesn't already exist.
        /// </summary>
        /// <param name="fileLayout">Details about the file that neets to pe present</param>
        /// <returns>returns the filename which is the FullName of the input type</returns>
        public async Task<string> EnsureFileAsync(FileLayout fileLayout)
        {
            if (fileLayout.Size == null) return null; 

            string org = GetFileName(fileLayout, true);
            if (!File.Exists(org)) return null; //original does not exist
            if (fileLayout.IsOriginal) return org; //Original is actually wanted

            string fn = GetFileName(fileLayout);
            if (File.Exists(fn))
                return fn; ///File laready cached return this
            
            return await Task.Run<string>(() =>
            {
                //using (Stream fl = File.OpenRead(org))
                //using (Stream output = File.OpenWrite(fn))
                //{
                    decimal fctr;
                using (Image<Rgba32> img = Image.Load(org))
                {
                    if (fileLayout.Size.MaxX <= 0 || fileLayout.Size.MaxY <= 0) // Use actual size but cahce
                        fctr = 1;
                    else
                    {
                        decimal xr = (decimal)img.Width / (decimal)fileLayout.Size.MaxX;
                        decimal yr = (decimal)img.Height / (decimal)fileLayout.Size.MaxY;
                        fctr = xr > yr ? xr : yr;
                    }
                    if (fctr > 1)
                        img.Resize((int)(img.Width / fctr), (int)(img.Height / fctr)).Save(fn);  //Save(output,Rgba32);
                    else
                        img.Save(fn);//Save(output); //Already smaller as requested size, save as is

                }
                return fn;
            });
        }

        public Size GetSize(string size)
        {
            if (size == null) return null;
            Size result;
            _sizes.TryGetValue(size.ToLower(), out result);
            return result;
        }

        public void AddSizes(IEnumerable<Size> sizes)
        {

            foreach (Size size in sizes)
                _sizes.Add(size.Name.ToLower(), size);
        }

        public IEnumerable<Size> Sizes {get { return _sizes.Values; } }


        string GetFileName(FileLayout fl, bool original = false)
        {
            if(original) return Path.Combine(_imageDirectory, fl.OriginalName); //There is no cache file if the original is been looked at
            
            return Path.Combine(_cacheDirectory, fl.Name);

        }

        /// <summary>
        /// Clear all files in the cache
        /// </summary>
        public void ClearCache()
        {
            //throw new NotImplementedException();
            System.IO.DirectoryInfo myDirInfo = new DirectoryInfo(CacheDirectory);

            foreach (FileInfo file in myDirInfo.GetFiles())
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Delete all files that are not in the dbRefs
        /// </summary>
        /// <param name="dbRefs">Files that are referenced.</param>
        public void CleanUp(IEnumerable<string> dbRefs)
        {
            CleanUp(new FileStats(this,dbRefs));
        }

        public void CleanUp(FileStats stats)
        {
            foreach(FileInfo fi in stats.NoDbRefs)
            {
                fi.Delete();
            }
        }

        public FileStats GetFileStats(IEnumerable<string> dbRefs)
        {
            return new FileStats(this,dbRefs);
        }
    }
}
