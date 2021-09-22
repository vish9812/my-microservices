using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShoppingController : ControllerBase
    {
        private readonly ICatalogService catalogService;
        private readonly IBasketService basketService;
        private readonly IOrderService orderService;

        public ShoppingController(ICatalogService catalogService, IBasketService basketService, IOrderService orderService)
        {
            this.catalogService = catalogService;
            this.basketService = basketService;
            this.orderService = orderService;
        }

        [HttpGet("{username}")]
        [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetShopping(string username)
        {
            // Send both queries in parallel
            var ordersTask = orderService.GetOrdersByUserName(username);
            var basket = await basketService.GetBasket(username);

            var itemsMap = basket.Items.ToDictionary(i => i.ProductId);
            var productTasks = basket.Items.Select(i => catalogService.GetCatalog(i.ProductId)).ToList();

            // Process items in parallel
            while (productTasks.Any())
            {
                var productTask = await Task.WhenAny(productTasks);

                productTasks.Remove(productTask);

                var product = await productTask;
                var item = itemsMap[product.Id];

                item.ProductName = product.Name;
                item.Category = product.Category;
                item.Summary = product.Summary;
                item.Description = product.Description;
                item.ImageFile = product.ImageFile;
            }

            var shoppingModel = new ShoppingModel
            {
                UserName = username,
                BasketWithProducts = basket,
                Orders = await ordersTask,
            };

            return Ok(shoppingModel);
        }
    }
}