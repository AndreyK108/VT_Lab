using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Khramtsevich_lab.UI.Areas.Admin.Pages
{
    [Authorize(Policy = "admin")]
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public List<Dish> Dishes { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;

        public async Task OnGetAsync(int? pageNo = 1)
        {
            var response = await _productService.GetProductListAsync(null, pageNo ?? 1);
            if (response.Success && response.Data != null)
            {
                Dishes = response.Data.Items ?? new();
                CurrentPage = response.Data.CurrentPage;
                TotalPages = response.Data.TotalPages;
            }
        }
    }
}
