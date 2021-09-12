using Basket.Api.Controllers.GrpcServices.Interfaces;
using Discount.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Api.Controllers.GrpcServices
{
    public class DiscountGrpcService : IDiscountService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient client;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient client)
        {
            this.client = client;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var request = new GetDiscountRequest { ProductName = productName };
            return await client.GetDiscountAsync(request);
        }
    }
}