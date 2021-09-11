using Basket.Api.Entities;
using Basket.Api.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Basket.Api.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache cache;

        public BasketRepository(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public Task DeleteBasket(string username)
        {
            return cache.RemoveAsync(username);
        }

        public async Task<ShoppingCart> GetBasket(string username)
        {
            var basket = await cache.GetStringAsync(username);

            if (string.IsNullOrEmpty(basket)) return null;

            return JsonSerializer.Deserialize<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));

            return basket;
        }
    }
}