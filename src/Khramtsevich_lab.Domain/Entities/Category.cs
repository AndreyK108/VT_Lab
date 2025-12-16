using System.Text.Json.Serialization;

namespace Khramtsevich_lab.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string NormalizedName { get; set; } = null!;
        public string? Description { get; set; }

        [JsonIgnore]
        public ICollection<Dish>? Dishes { get; set; }
    }
}
