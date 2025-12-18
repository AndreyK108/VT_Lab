using Khramtsevich_lab.API.Controllers;
using Khramtsevich_lab.API.Data;
using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Khramtsevich_lab.Tests
{
    public class DishApiControllerTests
    {
        private static async Task<AppDbContext> CreateDbAsync(SqliteConnection connection)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

            var db = new AppDbContext(options);
            await db.Database.EnsureCreatedAsync();
            return db;
        }

        private static async Task SeedAsync(AppDbContext db)
        {
            var soups = new Category { Name = "Супы", NormalizedName = "soups" };
            var salads = new Category { Name = "Салаты", NormalizedName = "salads" };

            db.Categories.AddRange(soups, salads);

            // 5 супов + 1 салат (для проверки фильтра и пагинации)
            db.Dishes.AddRange(
                new Dish { Name = "Суп1", Price = 10, Category = soups },
                new Dish { Name = "Суп2", Price = 11, Category = soups },
                new Dish { Name = "Суп3", Price = 12, Category = soups },
                new Dish { Name = "Суп4", Price = 13, Category = soups },
                new Dish { Name = "Суп5", Price = 14, Category = soups },
                new Dish { Name = "Салат1", Price = 20, Category = salads }
            );

            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task GetDishes_WhenCategoryHasNoItems_ReturnsSuccessFalse_AndErrorMessage()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            await using var db = await CreateDbAsync(connection);
            await SeedAsync(db);

            var env = Substitute.For<IWebHostEnvironment>();
            var controller = new DishesController(db, env);

            // Act
            var action = await controller.GetDishes(category: "desserts", pageNo: 1, pageSize: 3);
            var response = action.Value;

            // Assert
            Assert.NotNull(response);
            Assert.False(response!.Success);
            Assert.Equal("Нет объектов в выбранной категории", response.ErrorMessage);

            Assert.NotNull(response.Data);
            Assert.NotNull(response.Data!.Items);
            Assert.Empty(response.Data.Items);
            Assert.Equal(1, response.Data.CurrentPage);
            Assert.Equal(1, response.Data.TotalPages);
        }

        [Fact]
        public async Task GetDishes_FiltersByCategoryNormalizedName()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            await using var db = await CreateDbAsync(connection);
            await SeedAsync(db);

            var env = Substitute.For<IWebHostEnvironment>();
            var controller = new DishesController(db, env);

            // Act
            var action = await controller.GetDishes(category: "salads", pageNo: 1, pageSize: 10);
            var response = action.Value;

            // Assert
            Assert.NotNull(response);
            Assert.True(response!.Success);

            Assert.NotNull(response.Data);
            var items = response.Data!.Items;

            Assert.Single(items);
            Assert.Equal("Салат1", items[0].Name);

            // Include(d => d.Category) должен подтянуть категорию
            Assert.NotNull(items[0].Category);
            Assert.Equal("salads", items[0].Category!.NormalizedName);
        }

        [Fact]
        public async Task GetDishes_ClampsPageNo_ToTotalPages()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            await using var db = await CreateDbAsync(connection);
            await SeedAsync(db);

            var env = Substitute.For<IWebHostEnvironment>();
            var controller = new DishesController(db, env);

            // soups = 5 items, pageSize=2 => totalPages=3
            var action = await controller.GetDishes(category: "soups", pageNo: 999, pageSize: 2);
            var response = action.Value;

            Assert.NotNull(response);
            Assert.True(response!.Success);

            Assert.NotNull(response.Data);
            Assert.Equal(3, response.Data!.TotalPages);
            Assert.Equal(3, response.Data.CurrentPage);

            // последняя страница при 5 элементах и pageSize=2 => 1 элемент
            Assert.Single(response.Data.Items);
        }
    }
}
