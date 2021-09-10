using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Api.Configs
{
    public class DatabaseConfig
    {
        public const string DatabaseSettings = "DatabaseSettings";

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public CollectionNamesConfig CollectionNames { get; set; }
    }

    public class CollectionNamesConfig
    {
        public string Products { get; set; }
    }
}