using AutoMapper;
using Basket.Api.Controllers.GrpcServices.Interfaces;
using Basket.Api.Entities;
using Basket.Api.Repositories.Interfaces;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Basket.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository repository;
        private readonly IDiscountService discountService;
        private readonly IMapper mapper;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly ILogger<BasketController> logger;

        public BasketController(IBasketRepository repository, IDiscountService discountService, IMapper mapper, IPublishEndpoint publishEndpoint, ILogger<BasketController> logger)
        {
            this.repository = repository;
            this.discountService = discountService;
            this.mapper = mapper;
            this.publishEndpoint = publishEndpoint;
            this.logger = logger;
        }

        [HttpGet("{username}")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBasket(string username)
        {
            var basket = await repository.GetBasket(username);

            return Ok(basket ?? new ShoppingCart(username));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateBasket([FromBody] ShoppingCart basket)
        {
            //Info: Consuming other microservice Discount via gRPC
            foreach (var item in basket.Items)
            {
                var coupon = await discountService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }

            return Ok(await repository.UpdateBasket(basket));
        }

        [HttpDelete("{username}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string username)
        {
            await repository.DeleteBasket(username);

            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var basket = await repository.GetBasket(basketCheckout.UserName);

            if (basket == null)
            {
                return BadRequest();
            }

            var checkoutEvent = mapper.Map<BasketCheckoutEvent>(basketCheckout);
            checkoutEvent.TotalPrice = basket.TotalPrice;

            logger.LogInformation($"Publishing... event: {nameof(BasketCheckoutEvent)} with Id: {checkoutEvent.Id}");

            await publishEndpoint.Publish(checkoutEvent);

            logger.LogInformation($"Published event: {nameof(BasketCheckoutEvent)} with Id: {checkoutEvent.Id}");

            await repository.DeleteBasket(basket.UserName);

            logger.LogInformation($"Clearing basket for user: {basket.UserName}");

            return Accepted();
        }
    }
}