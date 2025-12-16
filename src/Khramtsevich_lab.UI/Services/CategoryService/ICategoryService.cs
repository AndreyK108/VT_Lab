using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Domain.Models;

namespace Khramtsevich_lab.Services
{
    public interface ICategoryService
    {
        /// Получение списка всех категорий
        public Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}