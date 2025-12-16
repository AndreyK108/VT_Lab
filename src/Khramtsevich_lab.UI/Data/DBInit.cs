using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Khramtsevich_lab.Data
{
    public class DbInit
    {
        public static async Task SeedData(WebApplication application)
        {
            // Создаем scope для получения сервисов
            using var scope = application.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Проверяем, существует ли пользователь с указанным email
            var user = await userManager.FindByEmailAsync("admin@gmail.com");
            if (user == null)
            {
                // Создаем нового пользователя
                user = new ApplicationUser
                {
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    EmailConfirmed = true
                };

                // Создаем пользователя с указанным паролем
                var result = await userManager.CreateAsync(user, "123456");
                if (result.Succeeded)
                {
                    // Добавляем утверждение "role" со значением "admin"
                    var claim = new Claim(ClaimTypes.Role, "admin");
                    await userManager.AddClaimAsync(user, claim);
                }
                else
                {
                    // Логируем ошибки, если создание пользователя не удалось
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }
        }
    }
}