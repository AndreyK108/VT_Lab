using Khramtsevich_lab.Domain.Models;
using Khramtsevich_lab.UI.Extensions;
using Microsoft.AspNetCore.Http;

namespace Khramtsevich_lab.UI.Models
{
    public class SessionCart : Cart
    {
        public static Cart GetCart(IServiceProvider services)
        {
            ISession session =
                services.GetRequiredService<IHttpContextAccessor>()
                        .HttpContext!.Session;

            Cart cart = session.Get<Cart>("Cart") ?? new Cart();

            session.Set("Cart", cart);
            return cart;
        }
    }
}
