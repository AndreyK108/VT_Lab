using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Khramtsevich_lab.UI.Areas.Admin.Pages
{
    [Authorize(Policy = "admin")]
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public EditModel(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        [BindProperty]
        public Dish Dish { get; set; } = new();

        [BindProperty]
        public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var response = await _productService.GetProductByIdAsync(id.Value);
            if (!response.Success || response.Data == null) return NotFound();

            Dish = response.Data;

            var categoryListData = await _categoryService.GetCategoryListAsync();
            ViewData["CategoryId"] = new SelectList(categoryListData.Data, "Id", "Name", Dish.CategoryId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            await _productService.UpdateProductAsync(Dish.Id, Dish, Image);
            return RedirectToPage("./Index");
        }
    }
}
