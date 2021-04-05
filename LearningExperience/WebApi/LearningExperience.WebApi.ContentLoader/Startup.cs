using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LearningExperience.WebApi.ContentLoader
{
    using System;

    using Microsoft.Extensions.FileProviders;

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(
                policy => policy.AddPolicy(
                    "CorsPolicy",
                    opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
                                   {
                                       FileProvider = new PhysicalFileProvider(Configuration["DocumentsSchemeFolder"]
                                                                               ?? throw new NullReferenceException("Configuration key - DocumentsSchemeFolder doesn't exist")),
                                       RequestPath = "/Documents"
                                   });
        }
    }
}
