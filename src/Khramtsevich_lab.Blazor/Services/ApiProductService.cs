using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Domain.Models;

namespace Khramtsevich_lab.Blazor.Services;

public class ApiProductService(HttpClient Http) : IProductService<Dish>
{
    private List<Dish>? _dishes;
    private int _currentPage = 1;
    private int _totalPages = 1;

    public IEnumerable<Dish>? Products => _dishes;
    public int CurrentPage => _currentPage;
    public int TotalPages => _totalPages;

    public event System.Action? ListChanged;

    public async Task GetProducts(int pageNo = 1, int pageSize = 3)
    {
        // базовый адрес уже будет установлен при регистрации HttpClient
        var uri = Http.BaseAddress!.AbsoluteUri;

        // параметры запроса
        var queryData = new Dictionary<string, string>
        {
            { "pageNo", pageNo.ToString() },
            { "pageSize", pageSize.ToString() }
        };

        var query = QueryString.Create(queryData);

        // отправить запрос
        var result = await Http.GetAsync(uri + query.Value);

        if (result.IsSuccessStatusCode)
        {
            var responseData =
                await result.Content.ReadFromJsonAsync<ResponseData<ProductListModel<Dish>>>();

            if (responseData?.Data != null)
            {
                _currentPage = responseData.Data.CurrentPage;
                _totalPages = responseData.Data.TotalPages;
                _dishes = responseData.Data.Items;
            }
            else
            {
                _dishes = null;
                _currentPage = 1;
                _totalPages = 1;
            }

            ListChanged?.Invoke();
        }
        else
        {
            _dishes = null;
            _currentPage = 1;
            _totalPages = 1;
            ListChanged?.Invoke();
        }
    }
}
