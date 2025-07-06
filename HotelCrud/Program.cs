using HotelCrud.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Configure Entity Framework Core with SQL Server
        var conString = builder.Configuration.GetConnectionString("conexion") ??
            throw new InvalidOperationException("Connection string 'conexion' not found.");

        builder.Services.AddDbContext<HotelCaliforniaDbContext>(options =>
            options.UseSqlServer(conString));

        var app = builder.Build();

        // Forzar cultura: yyyy-MM-dd para fechas HTML5 (input type="date")
        var cultureInfo = new CultureInfo("en-CA");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
