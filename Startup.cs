using Apex.RaspberryPiServer.Models;
using Apex.RaspberryPiServer.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Apex.RaspberryPiServer.Sensors;

namespace Apex.RaspberryPiServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SensorsDBContext>(options => options.UseSqlite("Data Source=sensors.db"));
            services.AddControllers();
            services.AddCors();
            services.AddSignalR();
            services.AddSingleton<ISensorService, SensorService>();
            services.AddSingleton<ICameraService, CameraService>();
            services.AddSingleton<ILedService, LedService>();
            services.Configure<RaspberryPiSettings>(Configuration.GetSection("RaspberryPiSettings"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseCors(builder => builder.WithOrigins("http://localhost:5000").AllowAnyHeader().AllowAnyMethod().AllowCredentials());

            app.UseFileServer();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SensorHub>("/sensor");
                endpoints.MapControllers();
            });
        }
    }
}
