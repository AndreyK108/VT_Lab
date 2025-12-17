using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Khramtsevich_lab.UI.Areas.Admin.Pages
{
    [Authorize(Policy = "admin")]
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CreateModel(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        [BindProperty]
        public Dish Dish { get; set; } = new();

        [BindProperty]
        public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var categoryListData = await _categoryService.GetCategoryListAsync();
            ViewData["CategoryId"] = new SelectList(categoryListData.Data, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _productService.CreateProductAsync(Dish, Image);
            return RedirectToPage("./Index");
        }
    }
}
