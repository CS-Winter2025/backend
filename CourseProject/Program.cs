using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CourseProject;
using CourseProject.Areas.Calendar.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Ensures the session cookie is accessible only by the server
    options.Cookie.IsEssential = true; // Required for GDPR compliance
});
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DBContext' not found.")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login"; // Redirect here if not authenticated
    });

// Add authorization services
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();
app.MapControllers();
app.UseStaticFiles();


//app.InitializeCalendarDatabase();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapAreaControllerRoute(
    name: "Employees",
    areaName: "Employees",
    pattern: "Employees/{action=Index}/{id?}",
    defaults: new { controller = "Employees" });
app.MapAreaControllerRoute(
    name: "Services",
    areaName: "Services",
    pattern: "{controller=Services}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapAreaControllerRoute(
    name: "Housing",
    areaName: "Housing",
    pattern: "{controller=Housing}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapAreaControllerRoute(
    name: "Charges",
    areaName: "Charges",
    pattern: "{controller=Invoices}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapAreaControllerRoute(
    name: "Calendar",
    areaName: "Calendar",
    pattern: "Calendar/{controller=EventSchedules}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "Employees",
    areaName: "Employees",
    pattern: "Employees/{action=Index}/{id?}",
    defaults: new { controller = "Employees" });
app.MapAreaControllerRoute(
    name: "Services",
    areaName: "Services",
    pattern: "{controller=Services}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapAreaControllerRoute(
    name: "Charges",
    areaName: "Charges",
    pattern: "{controller=Invoices}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
