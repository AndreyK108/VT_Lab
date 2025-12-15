using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Khramtsevich_lab.Services
{
    public class MemoryProductService : IProductService
    {
        private List<Dish> _dishes = new();
        private List<Category> _categories = new();
        private readonly IConfiguration _config;

        public MemoryProductService(IConfiguration config, ICategoryService categoryService)
        {
            _config = config;

            // Безопасно получаем категории (не полагаемся на .Result/.Data)
            var categoriesResponse = categoryService.GetCategoryListAsync()
                .GetAwaiter()
                .GetResult();

            _categories = categoriesResponse?.Data ?? new List<Category>();

            SetupData();
        }

        /// <summary>
        /// Инициализация списков
        /// </summary>
        private void SetupData()
        {
            // безопасно берём id категорий (если вдруг не найдены — ставим -1)
            int soupsId = _categories.FirstOrDefault(c => c.NormalizedName == "soups")?.Id ?? -1;
            int saladsId = _categories.FirstOrDefault(c => c.NormalizedName == "salads")?.Id ?? -1;
            int startersId = _categories.FirstOrDefault(c => c.NormalizedName == "starters")?.Id ?? -1;
            int drinksId = _categories.FirstOrDefault(c => c.NormalizedName == "drinks")?.Id ?? -1;
            int dessertsId = _categories.FirstOrDefault(c => c.NormalizedName == "desserts")?.Id ?? -1;

            _dishes = new List<Dish>
            {
                new Dish {Id = 1, Name="Суп-харчо",
                    Description="Острый суп с говядиной и рисом",
                    Weight= 200, Image="img/dishes/Харчо.webp",
                    Price=15, CategoryId= soupsId},

                new Dish { Id = 2, Name="Борщ",
                    Description="Без сметаны",
                    Price=15, Weight= 330, Image="img/dishes/Борщ.jpeg",
                    CategoryId= soupsId},

                new Dish { Id = 3, Name="Цезарь",
                    Description="Классический салат Цезарь с добавлением куриного филе",
                    Price=25, Weight=150, Image="img/dishes/Цезарь.jpeg",
                    CategoryId= saladsId},

                new Dish { Id = 4, Name="Оливье",
                    Description="Без горошка",
                    Price=12, Weight=200, Image="img/dishes/Оливье.jpeg",
                    CategoryId= saladsId},

                new Dish { Id = 5, Name="Брускетта",
                    Description="С авокадо и креветкой",
                    Price=16, Weight=120, Image="img/dishes/Брускетта.jpg",
                    CategoryId= startersId},

                new Dish { Id = 6, Name="Крылышки Баффало",
                    Description="Очень острые",
                    Price=26, Weight=250, Image="img/dishes/Крылышки.jpeg",
                    CategoryId= startersId},

                new Dish { Id = 7, Name="Лагман",
                    Description="С говядиной и овощами",
                    Weight=300, Image="img/dishes/Лагман.jpg",
                    Price=20, CategoryId= soupsId},

                new Dish { Id = 8, Name="Греческий салат",
                    Description="Со свежими овощами и фетой",
                    Price=150, Weight=180, Image="img/dishes/Греческий.webp",
                    CategoryId= saladsId},

                new Dish { Id = 9, Name="Креветки в кляре",
                    Description="С соусом тартар",
                    Price=25, Weight=220, Image="img/dishes/Креветки.jpeg",
                    CategoryId= startersId},

                new Dish { Id = 10, Name="Том Ям",
                    Description="Острый тайский суп с кокосовым молоком",
                    Price=30, Weight=280, Image="img/dishes/ТомЯм.webp",
                    CategoryId= soupsId},

                new Dish { Id = 11, Name="Салат с тунцом",
                    Description="Свежий салат с консервированным тунцом и овощами",
                    Price=20, Weight=190, Image="img/dishes/СалатСТунцом.jpg",
                    CategoryId= saladsId},

                new Dish { Id = 12, Name="Мини-бургеры",
                    Description="Небольшие бургеры с говяжьей котлетой и сыром",
                    Price=23, Weight=150, Image="img/dishes/МиниБургеры.jpeg",
                    CategoryId= startersId},

                new Dish { Id = 13, Name="Мисо-суп",
                    Description="Японский суп с тофу и водорослями",
                    Price=15, Weight=250, Image="img/dishes/МисоСуп.webp",
                    CategoryId= soupsId},

                new Dish {Id =14 , Name = "Пина Колада",
                    Description="Безалкогольный коктейль с ананасовым соком и кокосовым молоком",
                    Price=13, Weight=200, Image="img/dishes/ПинаКолада.jpg",
                    CategoryId= drinksId},

                new Dish {Id =15 , Name = "Мохито",
                    Description="Безалкогольный освежающий коктейль с мятой и лаймом",
                    Price=13, Weight=200, Image="img/dishes/Мохито.webp",
                    CategoryId= drinksId},

                new Dish {Id =16 , Name = "Чизкейк",
                    Description="Классический чизкейк с клубничным соусом",
                    Price=15, Weight=150, Image="img/dishes/Чизкейк.webp",
                    CategoryId= dessertsId},

                new Dish {Id =17 , Name = "Тирамису",
                    Description="Итальянский десерт с маскарпоне и кофе",
                    Price=16, Weight=150, Image="img/dishes/Тирамису.jpg",
                    CategoryId= dessertsId},

                new Dish {Id = 18 , Name = "Крем-суп из тыквы",
                    Description="Сливочный суп с тыквой и специями",
                    Price=13, Weight=300, Image="img/dishes/КремСупИзТыквы.jpg",
                    CategoryId= soupsId},

                new Dish {Id =19 , Name = "Лимонад",
                    Description="Домашний лимонад с мятой и лимоном",
                    Price=11, Weight=250, Image="img/dishes/Лимонад.jpeg",
                    CategoryId= drinksId},

                new Dish {Id=20, Name="Наполеон",
                    Description="Слоеный торт с заварным кремом",
                    Price=13, Weight=180, Image="img/dishes/Наполеон.jpeg",
                    CategoryId= dessertsId},

                new Dish{Id=21, Name="Айс-ти",
                    Description="Освежающий холодный чай с лимоном",
                    Price=10, Weight=250, Image="img/dishes/АйсТи.jpg",
                    CategoryId= drinksId},
            };
        }

        public Task<ResponseData<ProductListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var result = new ResponseData<ProductListModel<Dish>>();

            int? categoryId = null;
            if (!string.IsNullOrEmpty(categoryNormalizedName))
            {
                categoryId = _categories
                    .FirstOrDefault(c => c.NormalizedName == categoryNormalizedName)
                    ?.Id;
            }

            var data = _dishes
                .Where(d => categoryId == null || d.CategoryId == categoryId.Value)
                .ToList();

            int pageSize = _config.GetSection("ItemsPerPage").Get<int>();
            if (pageSize <= 0) pageSize = 3;

            int totalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
            if (totalPages < 1) totalPages = 1;

            if (pageNo < 1) pageNo = 1;
            if (pageNo > totalPages) pageNo = totalPages;

            var pageItems = data
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            result.Data = new ProductListModel<Dish>
            {
                Items = pageItems,
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            if (data.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Нет объектов в выбраннной категории";
            }

            return Task.FromResult(result);
        }

        public Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            var dish = _dishes.FirstOrDefault(d => d.Id == id);

            return Task.FromResult(new ResponseData<Dish>
            {
                Data = dish,
                Success = dish != null,
                ErrorMessage = dish == null ? "Блюдо не найдено" : null
            });
        }

        public Task<ResponseData<Dish>> CreateProductAsync(Dish dish, IFormFile? file)
        {
            dish.Id = _dishes.Count == 0 ? 1 : _dishes.Max(d => d.Id) + 1;
            _dishes.Add(dish);

            return Task.FromResult(new ResponseData<Dish>
            {
                Data = dish
            });
        }

        public Task<ResponseData<Dish>> UpdateProductAsync(int id, Dish dish, IFormFile? file)
        {
            var existing = _dishes.FirstOrDefault(d => d.Id == id);

            if (existing == null)
            {
                return Task.FromResult(new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = "Блюдо не найдено"
                });
            }

            existing.Name = dish.Name;
            existing.Description = dish.Description;
            existing.Weight = dish.Weight;
            existing.Image = dish.Image;
            existing.CategoryId = dish.CategoryId;
            existing.Price = dish.Price;

            return Task.FromResult(new ResponseData<Dish>
            {
                Data = existing
            });
        }

        public Task<ResponseData<bool>> DeleteProductAsync(int id)
        {
            var dish = _dishes.FirstOrDefault(d => d.Id == id);

            if (dish != null)
                _dishes.Remove(dish);

            return Task.FromResult(new ResponseData<bool>
            {
                Data = dish != null,
                Success = dish != null,
                ErrorMessage = dish == null ? "Блюдо не найдено" : null
            });
        }
    }
}
