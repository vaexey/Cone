using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Panel
{
    public class PanelServer
    {
        protected Context context;

        protected Thread? serverThread;

        protected WebApplication? app;

        public PanelServer(Context context)
        {
            this.context = context;
        }

        public async Task Run()
        {
            if (app is not null)
                throw new Exception("Cannot run another instance of PanelServer");

            var builder = GetBuilder();
            app = builder.Build();

            //app.MapControllers();



            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //            Path.Combine(builder.Environment.ContentRootPath, "static")
            //        ),
            //    RequestPath = "/static"
            //});
            app.MapGet("/cone/version", () =>
                Assembly.GetAssembly(typeof(PanelServer)).GetName().Version.ToString());

            app.MapGet("/cone/devices", () => 
                context.GetAllDevices());

            app.MapGet("/cone/entries", () =>
                context.GetAllEntries());

            app.MapGet("/cone/entries/bind/{id}", (string id) =>
                context.GetBindEntry(id)
                    is BindEntry b
                    ? Results.Ok(b)
                    : Results.NotFound());

            await app.RunAsync();
        }

        public async Task Stop()
        {
            if(app is not null)
            {
                await app.StopAsync();

                app = null;
            }
        }

        protected WebApplicationBuilder GetBuilder()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            var builder = WebApplication.CreateBuilder();

            //builder.Services.AddDirectoryBrowser();
            builder.Host.UseSerilog();
            builder.Environment.ContentRootPath = Path.Combine(currentDirectory, "./panel");

            //builder.Services.Configure<JsonOptions>(options =>
            //{
            //    options.SerializerOptions.TypeInfoResolver
            //});

            //builder.Services.AddDbContext<PanelContext>
            //    (opt => opt.UseInMemoryDatabase("t"));

            //builder.Services.AddControllers();
            //builder.Services.AddEndpointsApiExplorer();

            //builder.Services.AddScoped<Context>(p =>
            //{
            //    return p.getre
            //});

                    //.Configure(config => config.UseStaticFiles())
                    //.ConfigureWebHostDefaults()
                    //.UseWebRoot("panel/static");

            return builder;
        }
    }
}
