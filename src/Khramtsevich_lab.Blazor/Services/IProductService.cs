using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khramtsevich_lab.Blazor.Services;

public interface IProductService<T> where T : class
{
    // Сообщение о том, что список изменился (для перерисовки компонентов)
    event Action? ListChanged;

    // Список объектов
    IEnumerable<T>? Products { get; }

    // Номер текущей страницы
    int CurrentPage { get; }

    // Общее количество страниц
    int TotalPages { get; }

    // Получение списка объектов
    Task GetProducts(int pageNo = 1, int pageSize = 3);
}
