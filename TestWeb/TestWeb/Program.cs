using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestWeb.Data;
using TestWeb.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TestWebContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TestWebContext") ?? throw new InvalidOperationException("Connection string 'TestWebContext' not found.")));

//var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Login_Connection") ?? throw new InvalidOperationException("Connection string 'LoginContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// 대소문자구분
builder.Services.AddRouting(option =>
{
    option.LowercaseUrls = true;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromSeconds(30 * 60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    //SeedData.Initialize(services);
}

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

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
