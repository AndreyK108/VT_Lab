using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Khramtsevich_lab.Services
{
    public class ApiCategoryService : ICategoryService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public ApiCategoryService(HttpClient http, IConfiguration config)
        {
            _http = http;
        }

        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/Categories");
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseData<List<Category>>
                    {
                        Success = false,
                        ErrorMessage = $"API error: {(int)response.StatusCode} {response.ReasonPhrase}"
                    };
                }

                var data = await response.Content.ReadFromJsonAsync<ResponseData<List<Category>>>(_jsonOptions);

                return data ?? new ResponseData<List<Category>>
                {
                    Success = false,
                    ErrorMessage = "Пустой ответ от API (Categories)"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<List<Category>>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка запроса к API (Categories): {ex.Message}"
                };
            }
        }
    }
}
