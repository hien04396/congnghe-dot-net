using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OnlineStoreContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configure separate authentication schemes for Admin and Customer
// Set CustomerScheme as default since most of the site is for customers
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "CustomerScheme";
        options.DefaultChallengeScheme = "CustomerScheme";
    })
    .AddCookie("AdminScheme", options =>
    {
        options.LoginPath = "/admin";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "AdminAuth";
    })
    .AddCookie("CustomerScheme", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "CustomerAuth";
    });

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CartService>();

var app = builder.Build();

// Ensure database and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OnlineStoreContext>();
    DbInitializer.EnsureSeedData(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
