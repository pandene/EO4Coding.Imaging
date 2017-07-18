# EO4Coding.Imaging
Image Resizing and caching for ASP.Net core middleware

## Getting started
### Run unit test under test directory to see the concepts and ideas on how the ImageProvider works.
### The WebSample only return `Hello world` for all urls except for the ones that maps to the images url.
In my example `http://localhost:57734/images/Test1~small.jpg` will return Test1.jpg in the small size 
Makes sure that you have images available for this, the startup will try and copy Test1 to Test5 from a resource directory 4 paths up from the current image dir if they do not exist `..\..\..\..\EO4Codong.Imaging.Resources\Images\` (you can also download this repository)


### The image Provider register by default these sizes, but you can register your own
```
 new Size() {Name="original",MaxX=0,MaxY=0}, //use 0 not to change the original size, this is also registered under the original size and will return the image as is
 new Size() {Name="original-cached",MaxX=0,MaxY=0}, //this return the original size, but it wil go through the resizing process which in most cases result in a smaller file size for the same image size, the compression algorithm used seem to compress more/beter, use the test cases to compare size. Test1.jpg in my example reduced from 14MB to 2.5MB using the same size
 new Size() {Name="xlarge",MaxX=1900,MaxY=1200 },
 new Size() {Name="large",MaxX=1024,MaxY=768 },
 new Size() {Name="medium",MaxX=800,MaxY=600 },
 new Size() {Name="small",MaxX=300,MaxY=240 },
 new Size() {Name="thumbnail",MaxX=160,MaxY=160 }
```
