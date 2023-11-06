using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Core.OptionModels;
using AspNetCoreIdentityApp.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using AspNetCoreIdentityApp.Core.Models;
internal class Program
{
<<<<<<< Updated upstream
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;
=======
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
>>>>>>> Stashed changes

        services.AddAuthentication().AddFacebook(opts =>
        {
            opts.AppId = configuration["Authentication:Facebook:AppId"];
            opts.AppSecret = configuration["Authentication:Facebook:AppSecret"];
        });
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<IEmailService, EmailService>();

        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
        builder.Services.AddScoped<IMemberService, MemberService>();
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"), options => {
                options.MigrationsAssembly("AspNetCoreIdentityApp.Repository");
            });
        });

        builder.Services.Configure<SecurityStampValidatorOptions>(options=> options.ValidationInterval= TimeSpan.FromMinutes(30));

       
        builder.Services.AddIdentityWithExt();
        builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
        builder.Services.ConfigureApplicationCookie(opt =>
        {
            var cookieBuilder = new CookieBuilder();
            cookieBuilder.Name = "UAppCookie";

            opt.LoginPath = new PathString("/Home/Signin");
            opt.LogoutPath = new PathString("/Member/Logout");
            opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
            opt.Cookie = cookieBuilder;
            opt.ExpireTimeSpan = TimeSpan.FromDays(60);
            opt.SlidingExpiration = true;
        });

        //builder.Services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<AppDbContext>();

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

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}