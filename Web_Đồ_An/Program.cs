
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_Đồ_An.Models;
using Web_Đồ_An.Models.Entities;

namespace Web_Đồ_An
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
			var connectionString = builder.Configuration.GetConnectionString("BanhNgotTMConnection");
			builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(connectionString));

			//cấu hình sử dụng session
			builder.Services.AddDistributedMemoryCache();
			//dịch vụ HttpContextAccessor

			//Đăng ký dịch vụ cho HttpContext
			builder.Services.AddHttpContextAccessor();

			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(10);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
				options.Cookie.Name = ".Dev.Session";
			});


			//Identity
			builder.Services.AddIdentity<AppUserModel, IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


			builder.Services.Configure<IdentityOptions>(options =>
			{
				// Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 4;
				options.User.RequireUniqueEmail = true;
			});

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Admin/Account/Login";
                options.AccessDeniedPath = "/Admin/Account/Login";
                options.SlidingExpiration = true;
            });
            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			//sử dụng session đã khai báo
			app.UseSession();


			app.MapControllerRoute(
			   name: "areas",
			   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");




			app.Run();
		}
	}
}