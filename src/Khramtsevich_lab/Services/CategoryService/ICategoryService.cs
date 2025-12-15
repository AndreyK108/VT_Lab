using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Models;

namespace Khramtsevich_lab.Services
{
    public interface ICategoryService
    {
        /// Получение списка всех категорий
        public Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}