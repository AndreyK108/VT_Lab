using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Khramtsevich_lab.Data;
using System.Security.Claims;
using Khramtsevich_lab.Services;
using System.Globalization;
using Serilog;
using Khramtsevich_lab.UI.Middleware;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("be-BY");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// ====== Serilog (ЛР8, Задание 2) ======
var logPath = Path.Combine(builder.Environment.ContentRootPath, "logs", "log.txt");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Information(
    "UI started. ContentRootPath={ContentRootPath}. LogPath={LogPath}",
    builder.Environment.ContentRootPath,
    logPath
);
// =======================================

// ВАЖНО для TagHelper Pager
builder.Services.AddHttpContextAccessor();

// ====== Session (ЛР8) ======
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
// ===========================

builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7002/");
});

builder.Services.AddHttpClient<IProductService, ApiProductService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7002/");
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p =>
        p.RequireClaim(ClaimTypes.Role, "admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ====== Middleware FileLogger (ЛР8) ======
app.UseMiddleware<FileLoggerMiddleware>();
// ========================================

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

await DbInit.SeedData(app);

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
