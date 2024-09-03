
using GardenSeedShop.Web;
using GardenSeedShop.Web.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BestShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(36000);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


			builder.Services.AddTransient<IFeatureChecker, FeatureChecker>();
			builder.Services.AddTransient<IGardenShopEmailSender, GardenShopEmailSender>();

			var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapRazorPages();

            app.Run();
        }
    }
}
