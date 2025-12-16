using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Domain.Models;
using Microsoft.AspNetCore.Http;


// namespace Khramtsevich_lab.Services
// {
//     public interface IProductService
//     {

//         public Task<ResponseData<ProductListModel<Dish>>> GetProductListAsync(string?
//         categoryNormalizedName, int pageNo = 1);

//         public Task<ResponseData<Dish>> GetProductByIdAsync(int id);

//         /// Обновление объекта
//         /// <param name="id">Id изменяемомго объекта</param>
//         /// <param name="product">объект с новыми параметрами</param>
//         /// <param name="formFile">Файл изображения</param>
// Task<ResponseData<Dish>> UpdateProductAsync(int id, Dish dish, IFormFile? file);

//         /// Удаление объекта      
//         /// <param name="id">Id удаляемомго объекта</param>
// Task<ResponseData<bool>> DeleteProductAsync(int id);
//         /// Создание объекта
//         /// <param name="product">Новый объект</param>
//         /// <param name="formFile">Файл изображения</param>
//         public Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile);
//     }



namespace Khramtsevich_lab.Services
{
    public interface IProductService
    {
        Task<ResponseData<ProductListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1);

        Task<ResponseData<Dish>> GetProductByIdAsync(int id);

        Task<ResponseData<Dish>> CreateProductAsync(Dish dish, IFormFile? file);

        Task<ResponseData<Dish>> UpdateProductAsync(int id, Dish dish, IFormFile? file);

        Task<ResponseData<bool>> DeleteProductAsync(int id);
    }
}
