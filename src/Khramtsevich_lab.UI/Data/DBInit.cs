using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Khramtsevich_lab.Data
{
    public class DbInit
    {
        public static async Task SeedData(WebApplication application)
        {
            using var scope = application.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Кому выдаём права админа (твой текущий аккаунт)
            const string adminEmail = "xramcevich@gmail.com";

            // Можно оставить запасного админа
            const string fallbackAdminEmail = "admin@gmail.com";
            const string fallbackAdminPassword = "123456";

            // 1) Выдать admin аккаунту, если он есть
            await EnsureAdminClaim(userManager, adminEmail, createIfMissing: false);

            // 2) Оставить/создать запасного admin@gmail.com
            await EnsureAdminClaim(userManager, fallbackAdminEmail, createIfMissing: true, passwordIfCreate: fallbackAdminPassword);
        }

        private static async Task EnsureAdminClaim(
            UserManager<ApplicationUser> userManager,
            string email,
            bool createIfMissing,
            string? passwordIfCreate = null)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                if (!createIfMissing) return;

                user = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, passwordIfCreate ?? "123456");
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                        Console.WriteLine($"Seed user create error ({email}): {error.Description}");
                    return;
                }
            }

            // Добавляем claim admin, если его ещё нет
            var claims = await userManager.GetClaimsAsync(user);
            bool hasAdmin = claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "admin");
            if (!hasAdmin)
            {
                var addResult = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "admin"));
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                        Console.WriteLine($"Seed claim error ({email}): {error.Description}");
                }
            }
        }
    }
}
