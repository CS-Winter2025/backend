using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CourseProject;
using CourseProject.Areas.Calendar.Models;

var builder = WebApplication.CreateBuilder(args);

string connectionString;
var jawsdbUrl = Environment.GetEnvironmentVariable("JAWSDB_URL");

if (!string.IsNullOrEmpty(jawsdbUrl))
{
    var uri = new Uri(jawsdbUrl);
    var userInfo = uri.UserInfo.Split(':');

    connectionString = $"Server={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Uid={userInfo[0]};Pwd={userInfo[1]};";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("No MySQL connection string found.");
}

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081);  // Bind to any IP address on port 8080
});


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


app.UseAuthorization();

app.MapStaticAssets();

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
