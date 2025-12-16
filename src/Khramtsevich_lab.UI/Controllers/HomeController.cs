using Khramtsevich_lab.Models;
using Microsoft.AspNetCore.Mvc; // Добавлено для Controller и IActionResult
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Khramtsevich_lab.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["text"] = "Лабораторные работы по курсу ASP.NET Core MVC";

            var items = new List<ListDemo>
            {
                new ListDemo { Id = 1, Name = "Элемент 1" },
                new ListDemo { Id = 2, Name = "Элемент 2" },
                new ListDemo { Id = 3, Name = "Элемент 3" },
                new ListDemo { Id = 4, Name = "Элемент 4" }
            };

            var selectList = new SelectList(items, "Id", "Name"); // Создание SelectList для передачи в представление
            return View(selectList); 
        }
    }
}