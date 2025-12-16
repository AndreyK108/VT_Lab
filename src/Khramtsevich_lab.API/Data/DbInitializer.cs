using Khramtsevich_lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Khramtsevich_lab.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(WebApplication app)
        {
            // 1) Берём контекст как Scoped-сервис
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 2) Применяем миграции (на всякий)
            await context.Database.MigrateAsync();

            // 3) Если БД уже заполнена — выходим
            if (await context.Categories.AnyAsync() || await context.Dishes.AnyAsync())
                return;

            // 4) Базовый URL вашего API (ВАЖНО: порт должен совпадать с Khramtsevich_lab.API)
            // Обычно по методичке: https://localhost:7002/
            // Если у тебя другой https-порт — поправь здесь.
            var uri = "https://localhost:7002/";

            // 5) Категории (Id НЕ задаём!)
            var categories = new List<Category>
            {
                new Category { Name = "Стартеры", NormalizedName = "starters" },
                new Category { Name = "Салаты", NormalizedName = "salads" },
                new Category { Name = "Супы", NormalizedName = "soups" },
                new Category { Name = "Напитки", NormalizedName = "drinks" },
                new Category { Name = "Десерты", NormalizedName = "desserts" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            Category Cat(string normalized) =>
                categories.First(c => c.NormalizedName == normalized);

            // 6) Блюда (Id НЕ задаём!)
            // ВАЖНО: Image теперь должен быть URL вида https://localhost:7002/Images/Файл.jpg
            var dishes = new List<Dish>
            {
                new Dish { Name="Суп-харчо", Description="Острый суп с говядиной и рисом", Weight=200, Price=15,
                    Category = Cat("soups"), Image = uri + "Images/Харчо.webp" },

                new Dish { Name="Борщ", Description="Без сметаны", Weight=330, Price=15,
                    Category = Cat("soups"), Image = uri + "Images/Борщ.jpeg" },

                new Dish { Name="Цезарь", Description="Классический салат Цезарь с добавлением куриного филе", Weight=150, Price=25,
                    Category = Cat("salads"), Image = uri + "Images/Цезарь.jpeg" },

                new Dish { Name="Оливье", Description="Без горошка", Weight=200, Price=12,
                    Category = Cat("salads"), Image = uri + "Images/Оливье.jpeg" },

                new Dish { Name="Брускетта", Description="С авокадо и креветкой", Weight=120, Price=16,
                    Category = Cat("starters"), Image = uri + "Images/Брускетта.jpg" },

                new Dish { Name="Крылышки Баффало", Description="Очень острые", Weight=250, Price=26,
                    Category = Cat("starters"), Image = uri + "Images/Крылышки.jpeg" },

                new Dish { Name="Лагман", Description="С говядиной и овощами", Weight=300, Price=20,
                    Category = Cat("soups"), Image = uri + "Images/Лагман.jpg" },

                new Dish { Name="Греческий салат", Description="Со свежими овощами и фетой", Weight=180, Price=150,
                    Category = Cat("salads"), Image = uri + "Images/Греческий.webp" },

                new Dish { Name="Креветки в кляре", Description="С соусом тартар", Weight=220, Price=25,
                    Category = Cat("starters"), Image = uri + "Images/Креветки.jpeg" },

                new Dish { Name="Том Ям", Description="Острый тайский суп с кокосовым молоком", Weight=280, Price=30,
                    Category = Cat("soups"), Image = uri + "Images/ТомЯм.webp" },

                new Dish { Name="Салат с тунцом", Description="Свежий салат с консервированным тунцом и овощами", Weight=190, Price=20,
                    Category = Cat("salads"), Image = uri + "Images/СалатСТунцом.jpg" },

                new Dish { Name="Мини-бургеры", Description="Небольшие бургеры с говяжьей котлетой и сыром", Weight=150, Price=23,
                    Category = Cat("starters"), Image = uri + "Images/МиниБургеры.jpeg" },

                new Dish { Name="Мисо-суп", Description="Японский суп с тофу и водорослями", Weight=250, Price=15,
                    Category = Cat("soups"), Image = uri + "Images/МисоСуп.webp" },

                new Dish { Name="Пина Колада", Description="Безалкогольный коктейль с ананасовым соком и кокосовым молоком", Weight=200, Price=13,
                    Category = Cat("drinks"), Image = uri + "Images/ПинаКолада.jpg" },

                new Dish { Name="Мохито", Description="Безалкогольный освежающий коктейль с мятой и лаймом", Weight=200, Price=13,
                    Category = Cat("drinks"), Image = uri + "Images/Мохито.webp" },

                new Dish { Name="Чизкейк", Description="Классический чизкейк с клубничным соусом", Weight=150, Price=15,
                    Category = Cat("desserts"), Image = uri + "Images/Чизкейк.webp" },

                new Dish { Name="Тирамису", Description="Итальянский десерт с маскарпоне и кофе", Weight=150, Price=16,
                    Category = Cat("desserts"), Image = uri + "Images/Тирамису.jpg" },

                new Dish { Name="Крем-суп из тыквы", Description="Сливочный суп с тыквой и специями", Weight=300, Price=13,
                    Category = Cat("soups"), Image = uri + "Images/КремСупИзТыквы.jpg" },

                new Dish { Name="Лимонад", Description="Домашний лимонад с мятой и лимоном", Weight=250, Price=11,
                    Category = Cat("drinks"), Image = uri + "Images/Лимонад.jpeg" },

                new Dish { Name="Наполеон", Description="Слоеный торт с заварным кремом", Weight=180, Price=13,
                    Category = Cat("desserts"), Image = uri + "Images/Наполеон.jpeg" },

                new Dish { Name="Айс-ти", Description="Освежающий холодный чай с лимоном", Weight=250, Price=10,
                    Category = Cat("drinks"), Image = uri + "Images/АйсТи.jpg" },
            };

            await context.Dishes.AddRangeAsync(dishes);
            await context.SaveChangesAsync();
        }
    }
}
