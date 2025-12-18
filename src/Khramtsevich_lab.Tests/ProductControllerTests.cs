using System.Collections.Generic;
using System.Threading.Tasks;
using Khramtsevich_lab.Controllers;
using Khramtsevich_lab.Domain.Entities;
using Khramtsevich_lab.Domain.Models;
using Khramtsevich_lab.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Khramtsevich_lab.Tests
{
    public class ProductControllerTests
    {
        [Fact]
        public async Task Index_WhenServiceReturnsData_ReturnsViewWithThatModel_AndSetsViewBag()
        {
            // Arrange
            var productService = Substitute.For<IProductService>();
            var categoryService = Substitute.For<ICategoryService>();

            var listModel = new ProductListModel<Dish>
            {
                Items = new List<Dish>
                {
                    new Dish { Id = 10, Name = "Борщ", Price = 15, CategoryId = 1 },
                    new Dish { Id = 11, Name = "Харчо", Price = 15, CategoryId = 1 }
                },
                CurrentPage = 3,
                TotalPages = 7
            };

            productService
                .GetProductListAsync("soups", 3)
                .Returns(Task.FromResult(new ResponseData<ProductListModel<Dish>>
                {
                    Success = true,
                    Data = listModel
                }));

            categoryService
                .GetCategoryListAsync()
                .Returns(Task.FromResult(new ResponseData<List<Category>>
                {
                    Success = true,
                    Data = new List<Category>
                    {
                        new Category { Id = 1, Name = "Супы", NormalizedName = "soups" },
                        new Category { Id = 2, Name = "Салаты", NormalizedName = "salads" }
                    }
                }));

            var controller = new ProductController(productService, categoryService);

            // Act
            var result = await controller.Index(category: "soups", pageNo: 3);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductListModel<Dish>>(view.Model);

            Assert.Same(listModel, model);
            Assert.Equal(2, model.Items.Count);
            Assert.Equal(3, model.CurrentPage);
            Assert.Equal(7, model.TotalPages);

            var cats = Assert.IsType<List<Category>>(view.ViewData["Categories"]);
            Assert.Equal(2, cats.Count);

            Assert.Equal("soups", view.ViewData["SelectedCategory"]);
        }

        [Fact]
        public async Task Index_WhenCategoryServiceReturnsError_ReturnsNotFoundObjectResult()
        {
            // Arrange
            var productService = Substitute.For<IProductService>();
            var categoryService = Substitute.For<ICategoryService>();

            productService
                .GetProductListAsync(Arg.Any<string?>(), Arg.Any<int>())
                .Returns(Task.FromResult(new ResponseData<ProductListModel<Dish>>
                {
                    Success = true,
                    Data = new ProductListModel<Dish>
                    {
                        Items = new List<Dish>(),
                        CurrentPage = 1,
                        TotalPages = 1
                    }
                }));

            const string catError = "Категории недоступны";
            categoryService
                .GetCategoryListAsync()
                .Returns(Task.FromResult(new ResponseData<List<Category>>
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = catError
                }));

            var controller = new ProductController(productService, categoryService);

            // Act
            var result = await controller.Index(category: "soups", pageNo: 1);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(catError, notFound.Value);
        }

        [Fact]
        public async Task Index_WhenProductServiceReturnsError_ReturnsNotFoundObjectResult()
        {
            // Arrange
            var productService = Substitute.For<IProductService>();
            var categoryService = Substitute.For<ICategoryService>();

            const string dishError = "Нет объектов в выбранной категории";

            productService
                .GetProductListAsync(Arg.Any<string?>(), Arg.Any<int>())
                .Returns(Task.FromResult(new ResponseData<ProductListModel<Dish>>
                {
                    Success = false,
                    Data = null,
                    ErrorMessage = dishError
                }));

            // categoryService тут не важен, до него не дойдём
            var controller = new ProductController(productService, categoryService);

            // Act
            var result = await controller.Index(category: "soups", pageNo: 2);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(dishError, notFound.Value);
        }
    }
}
