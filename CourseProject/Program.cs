using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CourseProject;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DBContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

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
    name: "Charges",
    areaName: "Charges",
    pattern: "{controller=Invoices}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
