using Khramtsevich_lab.Domain.Entities;

namespace Khramtsevich_lab.Domain.Models
{
    public class CartItem
    {
        public Dish Item { get; set; } = default!;
        public int Qty { get; set; }
    }
}
