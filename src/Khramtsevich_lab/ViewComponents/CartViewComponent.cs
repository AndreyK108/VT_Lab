using Microsoft.AspNetCore.Mvc;

namespace Khramtsevich_lab.ViewComponents 
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke() 
        {
            return View();
        }
    }
}