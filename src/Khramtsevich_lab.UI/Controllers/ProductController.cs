using Microsoft.AspNetCore.Mvc;
using Khramtsevich_lab.Services;
using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Domain.Models;

namespace Khramtsevich_lab.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [Route("Catalog")]
        [Route("Catalog/{category}")]
        public async Task<IActionResult> Index(string? category, int pageNo = 1)
        {
            var dishesResponse = await _productService.GetProductListAsync(category, pageNo);
            if (dishesResponse == null || !dishesResponse.Success || dishesResponse.Data == null)
            {
                // Важно для ЛР8 (задание 3): при ошибке возвращаем NotFoundObjectResult
                return NotFound(dishesResponse?.ErrorMessage ?? "Ошибка получения списка блюд");
            }

            var categoriesResponse = await _categoryService.GetCategoryListAsync();
            if (categoriesResponse == null || !categoriesResponse.Success || categoriesResponse.Data == null)
            {
                return NotFound(categoriesResponse?.ErrorMessage ?? "Ошибка получения списка категорий");
            }

            // ViewBag пишет в ViewData, поэтому в тестах можно читать через ViewData
            ViewBag.Categories = categoriesResponse.Data;
            ViewBag.SelectedCategory = category;

            return View(dishesResponse.Data);
        }
    }
}
