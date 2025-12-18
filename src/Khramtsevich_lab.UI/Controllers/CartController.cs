using Microsoft.AspNetCore.Mvc;
using Khramtsevich_lab.Services;
using Khramtsevich_lab.Domain.Models;
using Khramtsevich_lab.UI.Extensions;

namespace Khramtsevich_lab.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;

        public CartController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl;

            var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            return View(cart);
        }

        [Route("[controller]/add/{id:int}")]
        public async Task<IActionResult> Add(int id, string returnUrl = "/")
        {
            var data = await _productService.GetProductByIdAsync(id);

            if (data.Success && data.Data != null)
            {
                var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
                cart.Add(data.Data);
                HttpContext.Session.Set("cart", cart);
            }

            return Redirect(returnUrl);
        }

        [Route("[controller]/remove/{id:int}")]
        public IActionResult Remove(int id, string returnUrl = "/")
        {
            var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            cart.Remove(id);
            HttpContext.Session.Set("cart", cart);

            return RedirectToAction(nameof(Index), new { returnUrl });
        }

        public IActionResult Clear(string returnUrl = "/")
        {
            var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            cart.Clear();
            HttpContext.Session.Set("cart", cart);

            return Redirect(returnUrl);
        }
    }
}
