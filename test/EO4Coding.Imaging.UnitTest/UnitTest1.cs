using ImageSharp;
using System;
using System.IO;
using Xunit;

namespace EO4Coding.Imaging.UnitTest
{
    public class TestImaging
    {


        EO4Coding.Imaging.ImageFactory imgf;// = new VaVroom.Imaging.ImageFactory();

        /// <summary>
        /// Directory.GetCurrentDirectory()  //curent dir
        /// AppDomain.CurrentDomain.BaseDirectory 
        /// System.Reflection.Assembly.GetExecutingAssembly().CodeBase  
        /// System.Reflection.Assembly.GetExecutingAssembly().Location
        /// Constructor ensures that 4 test images are available in the image direcotory for testing. Either make sure you have the 5 images available or checkout the Resource repository EO4Coding.Imaging.Resources. T
        /// his will automatically copy the images over to the test dir
        /// </summary>
        public TestImaging()
        {
           
            string resPath = @"..\..\..\..\..\..\EO4Codong.Imaging.Resources\Images\";            //string rootPath = System.Environment.CurrentDirectory;
            string imageDir = "image";
            if (!Directory.Exists(imageDir)) Directory.CreateDirectory(imageDir);
            string ti = Path.Combine(imageDir, "Test1.jpg");
            string tio =resPath+"Test1.jpg";
            if (!File.Exists(ti))
            {
                if (!File.Exists(tio)) throw new Exception("Could not load resource for testing :" + tio);
                File.Copy(tio, ti);
            }

            ti = Path.Combine(imageDir, "Test2.jpg");
            tio = Path.Combine(resPath, "Test2.jpg");
            if (!File.Exists(ti))
            {
                if (!File.Exists(tio)) throw new Exception("Could not load resource for testing :" + tio);
                File.Copy(tio, ti);
            }
            ti = Path.Combine(imageDir, "Test3.jpg");
            tio = Path.Combine(resPath , "Test3.jpg");
            if (!File.Exists(ti))
            {
                if (!File.Exists(tio)) throw new Exception("Could not load resource for testing :" + tio);
                File.Copy(tio, ti);
            }
            ti = Path.Combine(imageDir, "Test4.jpg");
            tio = Path.Combine(resPath, "Test4.jpg");
            if (!File.Exists(ti))
            {
                if (!File.Exists(tio)) throw new Exception("Could not load resource for testing :" + tio);
                File.Copy(tio, ti);
            }



            imgf = new ImageFactory("~/Images/Vehicles", "Image");


        }

        /// <summary>
        /// Test that the names for images works correctly
        /// </summary>
        [Fact]
        public void TestImageNaming()
        {
            Assert.Equal(@"Image\cache\Test1~small.jpg", imgf.GetActualFileName("Test1.jpg", "small"));
            Assert.Equal(@"Image\Test1.jpg", imgf.GetActualFileName("Test1.jpg", "small", true));
            Assert.Equal(@"Image\cache\Test1.jpg", imgf.GetActualFileName("Test1.jpg", "original", false)); // Might want the actual original file to return but one might say we want the 'original; not the same as the original seeing that the orignal size do have x,y limits
            Assert.Equal(@"Image\Test1.jpg", imgf.GetActualFileName("Test1.jpg", "original", true)); // Might want the actual original file to return but one might say we want the 'original; not the same as the original seeing that the orignal size do have x,y limits
            Assert.Equal("~/Images/Vehicles/Test1.jpg", imgf.GetFullUrlName("Test1.jpg", "original"));


            Assert.Equal("~/Images/Vehicles/Test1~small.jpg", imgf.GetFullUrlName("Images/Vehicle.whateverdude-just,throwthis away/Test1.jpg", "small"));
            FileLayout fl = new FileLayout(imgf, "Vehicle/1-20023.jpg");
            Assert.Equal(fl.FullName, imgf.ImageUrl + fl.OriginalName);
            fl = new FileLayout(imgf, "~/Images/Vehicle/1-20023~original.jpg");
            Assert.Equal(fl.Name, fl.OriginalName);
            Assert.Equal(fl.Name, "1-20023.jpg");
            fl = new FileLayout(imgf, "Images/Vehicle/1-20023~large.jpg");
            Assert.Equal(fl.FullName, "~/Images/Vehicles/1-20023~large.jpg");
            Assert.Equal(fl.OriginalName, "1-20023.jpg");
            Assert.Equal(fl.Size, imgf.GetSize("large"));
            fl = new FileLayout(imgf, "~/Images/Vehicle/1-20023.jpg", "original");
            Assert.Equal(fl.FullName, imgf.ImageUrl + fl.OriginalName);
            Assert.Equal(fl.Name, "1-20023.jpg");
            fl = new FileLayout(imgf, "~/Images/Vehicle/1-20023~small.jpg", "original");
            Assert.Equal(fl.FullName, imgf.ImageUrl + fl.OriginalName);
            Assert.Equal(fl.Name, "1-20023.jpg");
            fl = new FileLayout(imgf, "~/Images/Vehicle/1-20023~small.jpg", imgf.GetSize("large"));
            Assert.Equal(fl.Name, "1-20023~large.jpg");
            Assert.Equal(fl.OriginalName, "1-20023.jpg");
            Assert.Equal(fl.Size, imgf.GetSize("large"));
            fl = new FileLayout(imgf, "~/Images/Vehicle/1-20023~doesnotexistever.jpg");
            Assert.Equal(fl.Name, "1-20023~.jpg");
            Assert.Equal(fl.OriginalName, "1-20023.jpg");
            Assert.Null(fl.Size);


        }

        /// <summary>
        /// Test that resizing and caching works correctly
        /// </summary>
        [Fact]
        public void TestCachingAndResizing()
        {



            //Cleanup cache files first incase they already exist, we first want to make sure that new size is created then cached
            if (File.Exists("Image\\cache\\Test1~small.jpg"))
                File.Delete("Image\\cache\\Test1~small.jpg");
            FileLayout fl = new FileLayout(imgf, "Test1~small.jpg");
            string fn = imgf.EnsureFileAsync(fl).Result; //, this should have been a private/internal method but would not be able to test here
            Assert.Equal("Image\\cache\\Test1~small.jpg", fn); //Make sure the actual file name is as expected
            //using (FileStream stream = File.OpenRead(fn))
            using(Image<Rgba32> img = Image.Load(fn)) // now load the filename and check the sizes

            {
                //Image img = new Image(stream);
                EO4Coding.Imaging.Size sz = imgf.GetSize("small"); //Make sure it correlates to the small size registered in factory
                Assert.True(img.Width <= sz.MaxX);
                Assert.True(img.Height <= sz.MaxY);
                fn = imgf.EnsureFileAsync(fl).Result; //Will throw error if it tries to recreate as the file is currently open. This will test to make sure the factory does nog recreate the file since it is already cached

            }

            //Same as above test just different sizes
            if (File.Exists("Image\\cache\\Test4~thumbnail.jpg"))
                File.Delete("Image\\cache\\Test4~thumbnail.jpg");
            fl = new FileLayout(imgf, "Test4~thumbnail.jpg");
            fn = imgf.EnsureFileAsync(fl).Result;
            Assert.Equal("Image\\cache\\Test4~thumbnail.jpg", fn);
            using (Image<Rgba32> img = Image.Load(fn)) //FileStream stream = File.OpenRead(fn))
            {
                //Image img = new Image(stream);
                EO4Coding.Imaging.Size sz = imgf.GetSize("thumbnail");
                Assert.True(img.Width <= sz.MaxX);
                Assert.True(img.Height <= sz.MaxY);
                fn = imgf.EnsureFileAsync(fl).Result; //Will throw error if it tries to recreate

            }
            if (File.Exists("Image\\cache\\Test1~large.jpg"))
                File.Delete("Image\\cache\\Test1~large.jpg");
            fl = new FileLayout(imgf, "Test1~large.jpg");
            fn = imgf.EnsureFileAsync(fl).Result;
            Assert.Equal("Image\\cache\\Test1~large.jpg", fn);
            using (Image<Rgba32> img = Image.Load(fn)) //FileStream stream = File.OpenRead(fn))
            {
                //Image img = new Image(stream);
                EO4Coding.Imaging.Size sz = imgf.GetSize("large");
                Assert.True(img.Width <= sz.MaxX);
                Assert.True(img.Height <= sz.MaxY);
                fn = imgf.EnsureFileAsync(fl).Result; //Will throw error if it tries to recreate

            }
            if (File.Exists("Image\\cache\\Test1~xlarge.jpg"))
                File.Delete("Image\\cache\\Test1~xlarge.jpg");
            fl = new FileLayout(imgf, "Test1~xlarge.jpg");
            fn = imgf.EnsureFileAsync(fl).Result;
            Assert.Equal("Image\\cache\\Test1~xlarge.jpg", fn);
            using (Image<Rgba32> img = Image.Load(fn)) //FileStream stream = File.OpenRead(fn))
            {
                //Image img = new Image(stream);
                EO4Coding.Imaging.Size sz = imgf.GetSize("xlarge");
                Assert.True(img.Width <= sz.MaxX);
                Assert.True(img.Height <= sz.MaxY);
                fn = imgf.EnsureFileAsync(fl).Result; //Will throw error if it tries to recreate

            }
            if (File.Exists("Image\\cache\\Test1~original-cached.jpg"))
                File.Delete("Image\\cache\\Test1~original-cached.jpg");

            fl = new FileLayout(imgf, "Test1~original-cached.jpg");
            fn = imgf.EnsureFileAsync(fl).Result;
            Assert.Equal("Image\\cache\\Test1~original-cached.jpg", fn);
            using (Image<Rgba32> img = Image.Load(fn)) //FileStream stream = File.OpenRead(fn))
            {
                //Image img = new Image(stream);
                //VaVroom.Imaging.Size sz = imgf.GetSize("original-cached");
                //Assert.LessOrEqual(img.Width, sz.MaxX);
                //Assert.LessOrEqual(img.Height, sz.MaxY);
                fn = imgf.EnsureFileAsync(fl).Result; //Will throw error if it tries to recreate

            }



            fl = new FileLayout(imgf, "Test4.jpg", "original");
            fn = imgf.EnsureFileAsync(fl).Result;
            Assert.Equal("Image\\Test4.jpg", fn);
            Assert.False(File.Exists(@"Image\cahce\Test4.jpg")); //Make sure no caching happens on original file requests
            fl = new FileLayout(imgf, "Test4.jpg");
            fn = imgf.EnsureFileAsync(fl).Result;
            Assert.Equal("Image\\Test4.jpg", fn);
            fl = new FileLayout(imgf, "NotExist.jpg");
            fn = imgf.EnsureFileAsync(fl).Result;
            Assert.Null(fn);
            fl = new FileLayout(imgf, "NotExist~small.jpg");
            fn = imgf.EnsureFileAsync(fl).Result;
            Assert.Null(fn);
            fl = new FileLayout(imgf, "Test1~doesnetexistever.jpg");
            fn = imgf.EnsureFileAsync(fl).Result;
            Assert.Null(fn);

        }

    }

}
