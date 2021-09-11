using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Api.Configs
{
    public class DatabaseConfig
    {
        public const string DatabaseSettings = "DatabaseSettings";

        public string ConnectionString { get; set; }
    }
}