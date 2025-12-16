namespace Khramtsevich_lab.Domain.Entities
{
    public class Dish
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int? Weight { get; set; }

        public string? Image { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}
