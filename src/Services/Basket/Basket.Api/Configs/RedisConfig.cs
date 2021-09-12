using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Api.Configs
{
    public class RedisConfig
    {
        public const string CacheSettings = "CacheSettings";

        public string ConnectionString { get; set; }
    }
}