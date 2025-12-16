using System.Collections.Generic;

namespace Khramtsevich_lab.Domain.Models
{
    public class ProductListModel<T>
    {
        public List<T> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
