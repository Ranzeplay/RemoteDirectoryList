using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemoteDirectoryList.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tusdotnet;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Stores;

namespace RemoteDirectoryList
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddControllersWithViews();

            services.Configure<FormOptions>(options =>
            {
                // Set the limit to 4 GB
                options.MultipartBodyLengthLimit = 4294967296;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseWebSockets();

            app.UseRouting();

            app.UseAuthorization();

            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            app.UseTus(httpContext => new DefaultTusConfiguration
            {
                Store = new TusDiskStore(appSettings.UploadTempDirectory),
                UrlPath = "/api/upload",
                Events = new Events
                {
                    OnFileCompleteAsync = async eventContext =>
                    {
                        var file = await eventContext.GetFileAsync();
                        Dictionary<string, Metadata> metadata = await file.GetMetadataAsync(eventContext.CancellationToken);
                        var content = await file.GetContentAsync(eventContext.CancellationToken);

                        await CopyFileToTargetDirectory(content, metadata, appSettings);
                    },
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Files}/{action=Directory}/{id?}");
            });
        }

        private static async Task CopyFileToTargetDirectory(Stream content, Dictionary<string, Metadata> metadata, AppSettings appSettings)
        {
            var tab = appSettings.RootDirectoryTabs.FirstOrDefault(t => t.Id == metadata["tabId"].GetString(Encoding.UTF8));

            var path = metadata["path"].GetString(Encoding.UTF8).TrimStart('/');
            var filename = metadata["filename"].GetString(Encoding.UTF8);
            var absolutePath = Path.Combine(tab.AbsolutePath, path, filename);

            var targetFile = File.OpenWrite(absolutePath);
            await content.CopyToAsync(targetFile);
            targetFile.Close();
        }
    }
}
