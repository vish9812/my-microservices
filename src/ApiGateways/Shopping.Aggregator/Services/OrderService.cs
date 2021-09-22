﻿using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient client;

        public OrderService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName)
        {
            var response = await client.GetAsync($"/api/v1/Order/{userName}");
            return await response.ReadContentAs<List<OrderResponseModel>>();
        }
    }
}