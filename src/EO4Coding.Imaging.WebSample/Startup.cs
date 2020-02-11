using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace EO4Coding.Imaging.WebSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            EnsureImages(Path.Combine(env.WebRootPath, @"images\"));

            IImageProvider imgfactory = new ImageProvider("~/images/", Path.Combine(env.WebRootPath, @"images\"));
            app.Map("/images", (appBuilder) =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.Headers.Add("Content-Type", "image/jpeg");
                    await imgfactory.WriteFileAsync(imgfactory.GetFileLayout(context.Request.Path), context.Response.Body);
                });
            });

            app.UseStaticFiles();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    



        private void EnsureImages(string imageDir)
        {
            string resPath =Path.Combine(imageDir, @"..\..\..\..\..\EO4Coding.Images.Resources\Images\");            //string rootPath = System.Environment.CurrentDirectory;
            if (!Directory.Exists(resPath)) return;

                if (!Directory.Exists(imageDir)) Directory.CreateDirectory(imageDir);
            string ti = Path.Combine(imageDir, "Test1.jpg");
            string tio = resPath + "Test1.jpg";
            if (!File.Exists(ti))
            {
                if (!File.Exists(tio)) return;// throw new Exception("Could not load resource for testing :" + tio);
                File.Copy(tio, ti);
            }

            ti = Path.Combine(imageDir, "Test2.jpg");
            tio = Path.Combine(resPath, "Test2.jpg");
            if (!File.Exists(ti))
            {
                if (!File.Exists(tio)) return;// throw new Exception("Could not load resource for testing :" + tio);
                File.Copy(tio, ti);
            }
            ti = Path.Combine(imageDir, "Test3.jpg");
            tio = Path.Combine(resPath, "Test3.jpg");
            if (!File.Exists(ti))
            {
                if (!File.Exists(tio)) return;//throw new Exception("Could not load resource for testing :" + tio);
                File.Copy(tio, ti);
            }
            ti = Path.Combine(imageDir, "Test4.jpg");
            tio = Path.Combine(resPath, "Test4.jpg");
            if (!File.Exists(ti))
            {
                if (!File.Exists(tio)) return;//throw new Exception("Could not load resource for testing :" + tio);
                File.Copy(tio, ti);
            }


        }
    }
}
