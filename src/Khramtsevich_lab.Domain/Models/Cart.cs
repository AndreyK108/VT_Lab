using Khramtsevich_lab.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Khramtsevich_lab.Domain.Models
{
    public class Cart
    {
        // ВАЖНО: public get/set, чтобы Session(JSON) мог восстановить корзину
        public List<CartItem> Items { get; set; } = new();

        public void Add(Dish dish)
        {
            var item = Items.FirstOrDefault(i => i.Item.Id == dish.Id);

            if (item == null)
            {
                Items.Add(new CartItem
                {
                    Item = dish,
                    Qty = 1
                });
            }
            else
            {
                item.Qty++;
            }
        }

        public void Remove(int dishId)
        {
            Items.RemoveAll(i => i.Item.Id == dishId);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public int Count()
        {
            return Items.Sum(i => i.Qty);
        }

        public decimal TotalPrice()
        {
            return Items.Sum(i => i.Item.Price * i.Qty);
        }
    }
}
