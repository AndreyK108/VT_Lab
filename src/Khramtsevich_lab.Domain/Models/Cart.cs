using Khramtsevich_lab.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Khramtsevich_lab.Domain.Models
{
    public class Cart
    {
        private List<CartItem> _items = new();

        public IEnumerable<CartItem> Items => _items;

        public void Add(Dish dish)
        {
            var item = _items.FirstOrDefault(i => i.Item.Id == dish.Id);

            if (item == null)
            {
                _items.Add(new CartItem
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
            _items.RemoveAll(i => i.Item.Id == dishId);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public int Count()
        {
            return _items.Sum(i => i.Qty);
        }

        public decimal TotalPrice()
        {
            return _items.Sum(i => i.Item.Price * i.Qty);
        }
    }
}
