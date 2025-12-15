using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Models;

namespace Khramtsevich_lab.Services
{
    public class MemoryCategoryService : ICategoryService
    {
        public Task<ResponseData<List<Category>>>
        GetCategoryListAsync()
        {
            var categories = new List<Category>
    {
        new Category {Id=1, Name="Стартеры",NormalizedName="starters"},
        new Category {Id=2, Name="Салаты", NormalizedName="salads"},
        new Category {Id = 3, Name="Супы", NormalizedName="soups"},
        new Category {Id = 4, Name="Основные блюда", NormalizedName="maincourses"},
        new Category {Id = 5, Name="Десерты", NormalizedName="desserts"},
        new Category {Id = 6, Name="Напитки", NormalizedName="drinks"},


    };
            var result = new ResponseData<List<Category>>();
            result.Data = categories;
            return Task.FromResult(result);
        }
    }
}