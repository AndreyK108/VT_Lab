using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Khramtsevich_lab.Controllers
{
    [Route("Image")] // все методы в этом контроллере доступны по маршруту /Image/...
    public class ImageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ImageController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("GetAvatar")]
        public async Task<IActionResult> GetAvatar()
        {
            // Пользователь не аутентифицирован
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
                return File("/img/profile.png", "image/png");

            // Identity.Name может быть null или не быть email
            var email = User.Identity.Name;
            if (string.IsNullOrWhiteSpace(email))
                return File("/img/profile.png", "image/png");

            var user = await _userManager.FindByEmailAsync(email);
            if (user?.AvatarImage == null || user.AvatarImage.Length == 0)
                return File("/img/profile.png", "image/png");

            var contentType = string.IsNullOrWhiteSpace(user.AvatarContentType)
                ? "image/png"
                : user.AvatarContentType;

            return File(user.AvatarImage, contentType);
        }
    }
}
