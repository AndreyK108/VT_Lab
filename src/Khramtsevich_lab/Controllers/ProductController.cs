using Microsoft.AspNetCore.Mvc;
using Khramtsevich_lab.Services;
using Domain.Entities;
using Domain.Models;

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

        // ✅ Задание 6: маршруты /Catalog и /Catalog/{category}
        [Route("Catalog")]
        [Route("Catalog/{category}")]
        public async Task<IActionResult> Index(string? category, int pageNo = 1)
        {
            var dishesResponse = await _productService.GetProductListAsync(category, pageNo);
            var categoriesResponse = await _categoryService.GetCategoryListAsync();

            ViewBag.Categories = categoriesResponse?.Data ?? new List<Category>();
            ViewBag.SelectedCategory = category;

            // Важно: в View передаём ProductListModel<Dish>, а не IEnumerable<Dish>
            if (dishesResponse?.Data == null)
            {
                // чтобы View не падал
                return View(new ProductListModel<Dish>
                {
                    Items = new List<Dish>(),
                    CurrentPage = 1,
                    TotalPages = 1
                });
            }

            return View(dishesResponse.Data);
        }
    }
}
