using Microsoft.AspNetCore.Mvc;
using Khramtsevich_lab.Domain.Models;
using Khramtsevich_lab.UI.Extensions;

namespace Khramtsevich_lab.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            return View(cart);
        }
    }
}
