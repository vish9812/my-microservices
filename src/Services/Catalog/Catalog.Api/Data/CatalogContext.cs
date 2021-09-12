using Catalog.Api.Configs;
using Catalog.Api.Data.Interfaces;
using Catalog.Api.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Api.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(IOptions<DatabaseConfig> dbOptions)
        {
            var dbConfig = dbOptions.Value;

            var client = new MongoClient(dbConfig.ConnectionString);
            var database = client.GetDatabase(dbConfig.DatabaseName);

            Products = database.GetCollection<Product>(dbConfig.CollectionNames.Products);

            CatalogContextSeed.SeedData(Products);
        }

        public IMongoCollection<Product> Products { get; }
    }
}