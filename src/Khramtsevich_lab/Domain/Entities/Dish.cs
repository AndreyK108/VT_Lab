namespace Domain.Entities
{
    public class Dish
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int? Weight { get; set; }

        // Путь к файлу изображения блюда
        public string? Image { get; set; }

        public int CategoryId { get; set; }

        // Навигационное свойство - категория блюда
        public Category? Category { get; set; }
    }
}
