using Xunit;
using CartApi.Services;
using CartApi.Models;
using CartApi.Tests.Mocks;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;
using System;

namespace CartApi.Tests
{
    public class CartServiceTests
    {
        private readonly CartService _service;
        private readonly MockCartRepository _repo;
        private readonly Mock<IDistributedCache> _cacheMock;

        public CartServiceTests()
        {
            _repo = new MockCartRepository();

            _cacheMock = new Mock<IDistributedCache>();

            // Mock GetAsync retornando null (cache vazio)
            _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), default))
                      .ReturnsAsync((byte[])null);

            // Mock SetAsync apenas completando a tarefa
            _cacheMock.Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                default))
                      .Returns(Task.CompletedTask);

            // Mock RemoveAsync apenas completando a tarefa
            _cacheMock.Setup(c => c.RemoveAsync(It.IsAny<string>(), default))
                      .Returns(Task.CompletedTask);

            _service = new CartService(_repo, _cacheMock.Object);
        }

        [Fact]
        public async Task AddItem_Should_Add_Item_To_Cart()
        {
            var item = new CartItem { CartId = 1, ProductId = 1, Quantity = 2 };
            await _service.AddItemAsync(item);

            var (items, total) = await _service.GetCartAsync(1);

            Assert.Single(items);
            var addedItem = items.First();

            Assert.Equal(1, addedItem.ProductId);
            Assert.Equal(2, addedItem.Quantity);
            Assert.Equal(10m, addedItem.Price); // preço mockado no MockCartRepository
        }

        [Fact]
        public async Task RemoveItem_Should_Remove_Item_From_Cart()
        {
            var item = new CartItem { CartId = 1, ProductId = 1, Quantity = 2 };
            await _service.AddItemAsync(item);

            await _service.RemoveItemAsync(1, 1);

            // Agora esperamos que o carrinho esteja vazio e lance KeyNotFoundException
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                var result = await _service.GetCartAsync(1);
            });
        }

        [Fact]
        public async Task ApplyDiscount_Should_Reduce_Total()
        {
            var item = new CartItem { CartId = 1, ProductId = 1, Quantity = 2 };
            await _service.AddItemAsync(item);

            var discount = new Discount { CartId = 1, Percentual = 0.1m };
            await _service.ApplyDiscountAsync(discount);

            var (items, total) = await _service.GetCartAsync(1);

            Assert.Equal(18m, total); // 20 - 10%
        }

        [Fact]
        public async Task GetCartTotal_Should_Return_Correct_Total()
        {
            var item = new CartItem { CartId = 1, ProductId = 1, Quantity = 2 };
            await _service.AddItemAsync(item);

            var (items1, total1) = await _service.GetCartAsync(1);
            var (items2, total2) = await _service.GetCartAsync(1);

            Assert.Equal(20m, total2);
            Assert.Single(items2);
        }
    }
}
