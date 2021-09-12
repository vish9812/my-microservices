using Dapper;
using Discount.Grpc.Configs;
using Discount.Grpc.Entities;
using Discount.Grpc.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly DatabaseConfig dbConfig;

        public DiscountRepository(IOptions<DatabaseConfig> dbConfigOptions)
        {
            dbConfig = dbConfigOptions.Value;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(dbConfig.ConnectionString);

            var query = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)";
            var affected = await connection.ExecuteAsync(query, new { coupon.ProductName, coupon.Description, coupon.Amount });

            return affected > 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(dbConfig.ConnectionString);

            var query = "DELETE FROM Coupon WHERE ProductName = @ProductName";
            var affected = await connection.ExecuteAsync(query, new { ProductName = productName });

            return affected > 0;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(dbConfig.ConnectionString);

            var query = "select * from Coupon where ProductName = @ProductName";
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(query, new { ProductName = productName });

            return coupon ?? new Coupon
            {
                ProductName = productName,
                Description = "No Discount"
            };
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(dbConfig.ConnectionString);

            var query = "UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id";
            var affected = await connection.ExecuteAsync(query, new { coupon.Id, coupon.ProductName, coupon.Description, coupon.Amount });

            return affected > 0;
        }
    }
}