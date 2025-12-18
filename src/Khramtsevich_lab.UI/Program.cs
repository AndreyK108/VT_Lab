using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Khramtsevich_lab.Data;
using System.Security.Claims;
using Khramtsevich_lab.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("be-BY");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

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

// Add services to the container.
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

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

// ====== Session (ЛР8) ======
app.UseSession();
// ===========================

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
app.Run();
