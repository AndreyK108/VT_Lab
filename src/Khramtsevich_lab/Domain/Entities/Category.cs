using System.Collections.Generic;

namespace Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string NormalizedName { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Навигационное свойство - список блюд в этой категории
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }
}
