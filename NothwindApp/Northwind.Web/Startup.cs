using static System.Console;
using Packt.Shared;

namespace Northwind.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddNorthwindContext();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseEndpoints(e => {
                e.MapRazorPages();
                e.MapGet("/hello", () => "Hello world!");
                }
            );
        }
    }
}
