using Khramtsevich_lab.Blazor.Components;
using Khramtsevich_lab.Blazor.Services;
using Khramtsevich_lab.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient + сервис продуктов (API)
builder.Services.AddHttpClient<IProductService<Dish>, ApiProductService>(client =>
{
    // адрес твоего API из launchSettings.json
    client.BaseAddress = new Uri("https://localhost:7002/api/dishes");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
