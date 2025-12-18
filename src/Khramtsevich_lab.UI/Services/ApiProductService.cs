using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Khramtsevich_lab.Services
{
    public class ApiProductService : IProductService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public ApiProductService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<ResponseData<ProductListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            int pageSize = _config.GetSection("ItemsPerPage").Get<int>();
            if (pageSize <= 0) pageSize = 3;

            var url = $"api/Dishes?pageNo={pageNo}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(categoryNormalizedName))
                url += $"&category={Uri.EscapeDataString(categoryNormalizedName)}";

            try
            {
                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseData<ProductListModel<Dish>>
                    {
                        Success = false,
                        ErrorMessage = $"API error: {(int)response.StatusCode} {response.ReasonPhrase}"
                    };
                }

                var data = await response.Content.ReadFromJsonAsync<ResponseData<ProductListModel<Dish>>>(_jsonOptions);

                return data ?? new ResponseData<ProductListModel<Dish>>
                {
                    Success = false,
                    ErrorMessage = "Пустой ответ от API (Dishes list)"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<ProductListModel<Dish>>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка запроса к API (Dishes list): {ex.Message}"
                };
            }
        }

        public async Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _http.GetAsync($"api/Dishes/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseData<Dish>
                    {
                        Success = false,
                        ErrorMessage = $"API error: {(int)response.StatusCode} {response.ReasonPhrase}"
                    };
                }

                // В разных реализациях API может вернуть либо Dish, либо ResponseData<Dish>.
                // Делаем безопасно: пробуем оба варианта.
                var json = await response.Content.ReadAsStringAsync();

                // 1) пробуем как ResponseData<Dish>
                try
                {
                    var wrapper = JsonSerializer.Deserialize<ResponseData<Dish>>(json, _jsonOptions);
                    if (wrapper != null && wrapper.Success && wrapper.Data != null)
                        return wrapper;
                }
                catch { /* ignore */ }

                // 2) пробуем как Dish
                try
                {
                    var dish = JsonSerializer.Deserialize<Dish>(json, _jsonOptions);
                    return dish == null
                        ? new ResponseData<Dish> { Success = false, ErrorMessage = "Пустой ответ от API (Dish by id)" }
                        : new ResponseData<Dish> { Success = true, Data = dish };
                }
                catch (Exception ex2)
                {
                    return new ResponseData<Dish>
                    {
                        Success = false,
                        ErrorMessage = $"Не удалось разобрать ответ API (Dish by id): {ex2.Message}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка запроса к API (Dish by id): {ex.Message}"
                };
            }
        }

        public async Task<ResponseData<Dish>> CreateProductAsync(Dish dish, IFormFile? file)
        {
            try
            {
                var createResponse = await _http.PostAsJsonAsync("api/Dishes", dish);
                if (!createResponse.IsSuccessStatusCode)
                {
                    return new ResponseData<Dish>
                    {
                        Success = false,
                        ErrorMessage = $"API error: {(int)createResponse.StatusCode} {createResponse.ReasonPhrase}"
                    };
                }

                var createdDish = await createResponse.Content.ReadFromJsonAsync<Dish>(_jsonOptions);
                if (createdDish == null)
                    return new ResponseData<Dish> { Success = false, ErrorMessage = "Пустой ответ от API (Create dish)" };

                if (file != null && file.Length > 0)
                {
                    using var content = new MultipartFormDataContent();
                    using var streamContent = new StreamContent(file.OpenReadStream());
                    content.Add(streamContent, "image", file.FileName);

                    var imgResponse = await _http.PostAsync($"api/Dishes/{createdDish.Id}", content);
                    if (!imgResponse.IsSuccessStatusCode)
                    {
                        return new ResponseData<Dish>
                        {
                            Success = false,
                            ErrorMessage = $"Блюдо создано, но изображение не загрузилось: {(int)imgResponse.StatusCode} {imgResponse.ReasonPhrase}",
                            Data = createdDish
                        };
                    }

                    var refreshed = await GetProductByIdAsync(createdDish.Id);
                    if (refreshed.Success && refreshed.Data != null)
                        createdDish = refreshed.Data;
                }

                return new ResponseData<Dish> { Success = true, Data = createdDish };
            }
            catch (Exception ex)
            {
                return new ResponseData<Dish> { Success = false, ErrorMessage = $"Ошибка запроса к API (Create dish): {ex.Message}" };
            }
        }

        public async Task<ResponseData<Dish>> UpdateProductAsync(int id, Dish dish, IFormFile? file)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"api/Dishes/{id}", dish);
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseData<Dish>
                    {
                        Success = false,
                        ErrorMessage = $"API error: {(int)response.StatusCode} {response.ReasonPhrase}"
                    };
                }

                if (file != null && file.Length > 0)
                {
                    using var content = new MultipartFormDataContent();
                    using var streamContent = new StreamContent(file.OpenReadStream());
                    content.Add(streamContent, "image", file.FileName);

                    var imgResponse = await _http.PostAsync($"api/Dishes/{id}", content);
                    if (!imgResponse.IsSuccessStatusCode)
                    {
                        return new ResponseData<Dish>
                        {
                            Success = false,
                            ErrorMessage = $"Объект обновлён, но изображение не загрузилось: {(int)imgResponse.StatusCode} {imgResponse.ReasonPhrase}"
                        };
                    }
                }

                return await GetProductByIdAsync(id);
            }
            catch (Exception ex)
            {
                return new ResponseData<Dish> { Success = false, ErrorMessage = $"Ошибка запроса к API (Update dish): {ex.Message}" };
            }
        }

        public async Task<ResponseData<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/Dishes/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseData<bool>
                    {
                        Success = false,
                        ErrorMessage = $"API error: {(int)response.StatusCode} {response.ReasonPhrase}"
                    };
                }

                return new ResponseData<bool> { Success = true, Data = true };
            }
            catch (Exception ex)
            {
                return new ResponseData<bool> { Success = false, Data = false, ErrorMessage = $"Ошибка запроса к API (Delete dish): {ex.Message}" };
            }
        }
    }
}
