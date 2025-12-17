using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Khramtsevich_lab.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Khramtsevich_lab.UI.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class AvatarModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AvatarModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        [Display(Name = "Новый аватар")]
        public IFormFile? Avatar { get; set; }

        public string CurrentAvatarUrl => "/Image/GetAvatar";

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Avatar == null || Avatar.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Выберите файл изображения.");
                return Page();
            }

            // Небольшая валидация по типу
            if (string.IsNullOrWhiteSpace(Avatar.ContentType) || !Avatar.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError(string.Empty, "Файл должен быть изображением.");
                return Page();
            }

            // Ограничим размер, чтобы не раздувать БД (можно поменять)
            const int maxBytes = 2 * 1024 * 1024; // 2 MB
            if (Avatar.Length > maxBytes)
            {
                ModelState.AddModelError(string.Empty, "Слишком большой файл (максимум 2 MB).");
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            await using var ms = new MemoryStream();
            await Avatar.CopyToAsync(ms);

            user.AvatarImage = ms.ToArray();
            user.AvatarContentType = Avatar.ContentType;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);
                return Page();
            }

            // Обновим куки, чтобы всё подтянулось корректно
            await _signInManager.RefreshSignInAsync(user);

            TempData["StatusMessage"] = "Аватар обновлён.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            user.AvatarImage = null;
            user.AvatarContentType = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["StatusMessage"] = "Аватар удалён.";
            return RedirectToPage();
        }
    }
}
