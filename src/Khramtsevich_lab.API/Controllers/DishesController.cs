using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khramtsevich_lab.API.Data;
using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Khramtsevich_lab.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DishesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Dishes?category=soups&pageNo=1&pageSize=3
        [HttpGet]
        public async Task<ActionResult<ResponseData<ProductListModel<Dish>>>> GetDishes(
            string? category,
            int pageNo = 1,
            int pageSize = 3)
        {
            var result = new ResponseData<ProductListModel<Dish>>();

            var query = _context.Dishes
                .Include(d => d.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(d => d.Category != null && d.Category.NormalizedName == category);
            }

            var count = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            if (totalPages == 0) totalPages = 1;
            if (pageNo < 1) pageNo = 1;
            if (pageNo > totalPages) pageNo = totalPages;

            var items = await query
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            result.Data = new ProductListModel<Dish>
            {
                Items = items,
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            if (count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Нет объектов в выбранной категории";
            }

            return result;
        }

        // GET: api/Dishes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dish>> GetDish(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dish == null)
                return NotFound();

            return dish;
        }

        // PUT: api/Dishes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDish(int id, Dish dish)
        {
            if (id != dish.Id)
                return BadRequest();

            _context.Entry(dish).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DishExists(id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // POST: api/Dishes
        [HttpPost]
        public async Task<ActionResult<Dish>> PostDish(Dish dish)
        {
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDish), new { id = dish.Id }, dish);
        }

        // POST: api/Dishes/{id}  (сохранение изображения)
        [HttpPost("{id}")]
        public async Task<IActionResult> SaveImage(int id, IFormFile image)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
                return NotFound();

            if (image == null || image.Length == 0)
                return BadRequest("Файл не передан");

            // wwwroot/Images
            var imagesPath = Path.Combine(_env.WebRootPath, "Images");
            Directory.CreateDirectory(imagesPath);

            // случайное имя + расширение оригинала
            var randomName = Path.GetRandomFileName();
            var extension = Path.GetExtension(image.FileName);
            var fileName = Path.ChangeExtension(randomName, extension);
            var filePath = Path.Combine(imagesPath, fileName);

            using (var stream = System.IO.File.OpenWrite(filePath))
            {
                await image.CopyToAsync(stream);
            }

            // абсолютный URL на файл
            var url = $"{Request.Scheme}://{Request.Host}/Images/{fileName}";

            dish.Image = url;
            await _context.SaveChangesAsync();

            return Ok(url);
        }

        // DELETE: api/Dishes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
                return NotFound();

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.Id == id);
        }
    }
}
