using Xunit;
using Moq;
using CartApi.Controllers;
using CartApi.Services;
using CartApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartApi.Tests
{
    public class CartControllerTests
    {
        private readonly Mock<CartService> _serviceMock;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _serviceMock = new Mock<CartService>(MockBehavior.Strict, null, null);
            _controller = new CartController(_serviceMock.Object);
        }

        [Fact]
        public async Task CreateCart_Should_Return_CartId()
        {
            _serviceMock.Setup(s => s.CreateCartAsync("user1")).ReturnsAsync(1);

            var result = await _controller.CreateCart("user1") as OkObjectResult;
            Assert.NotNull(result);

            var json = System.Text.Json.JsonSerializer.Serialize(result.Value);
            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string,int>>(json);
            
            Assert.NotNull(data);
            Assert.True(data.ContainsKey("CartId"));
            Assert.Equal(1, data["CartId"]);
        }

        [Fact]
        public async Task GetCart_Should_Return_Cart()
        {
            var items = new List<CartItemDto>
            {
                new CartItemDto { ProductId = 1, Quantity = 2, Price = 10, Name = "Product A" }
            };

            _serviceMock.Setup(s => s.GetCartAsync(1)).ReturnsAsync((items, 20m));

            var result = await _controller.GetCart(1) as OkObjectResult;
            Assert.NotNull(result);

            var tuple = ((List<CartItemDto> Items, decimal Total))result.Value;

            Assert.Equal(20m, tuple.Total);
            Assert.Single(tuple.Items);
        }

        [Fact]
        public async Task AddItem_Should_Call_Service()
        {
            var item = new CartItem { CartId = 1, ProductId = 1, Quantity = 2 };

            _serviceMock.Setup(s => s.AddItemAsync(It.Is<CartItem>(c =>
                    c.CartId == 1 && c.ProductId == 1 && c.Quantity == 2)))
                        .Returns(Task.CompletedTask);

            var result = await _controller.AddItem(1, item) as OkObjectResult;
            Assert.NotNull(result);

            _serviceMock.Verify(s => s.AddItemAsync(It.Is<CartItem>(c =>
                    c.CartId == 1 && c.ProductId == 1 && c.Quantity == 2)), Times.Once);
        }

        [Fact]
        public async Task RemoveItem_Should_Call_Service()
        {
            _serviceMock.Setup(s => s.RemoveItemAsync(1, 1)).Returns(Task.CompletedTask);

            var result = await _controller.RemoveItem(1, 1) as OkObjectResult;
            Assert.NotNull(result);

            _serviceMock.Verify(s => s.RemoveItemAsync(1, 1), Times.Once);
        }

        [Fact]
        public async Task ApplyDiscount_Should_Call_Service()
        {
            var discount = new Discount { CartId = 1, Percentual = 0.1m };

            _serviceMock.Setup(s => s.ApplyDiscountAsync(It.Is<Discount>(d =>
                    d.CartId == 1 && d.Percentual == 0.1m)))
                        .Returns(Task.CompletedTask);

            var result = await _controller.ApplyDiscount(1, discount) as OkObjectResult;
            Assert.NotNull(result);

            _serviceMock.Verify(s => s.ApplyDiscountAsync(It.Is<Discount>(d =>
                    d.CartId == 1 && d.Percentual == 0.1m)), Times.Once);
        }
    }
}
