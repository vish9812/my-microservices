using Discount.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Api.Controllers.GrpcServices.Interfaces
{
    public interface IDiscountService
    {
        Task<CouponModel> GetDiscount(string productName);
    }
}