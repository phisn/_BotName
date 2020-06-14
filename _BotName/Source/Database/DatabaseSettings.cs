using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;

namespace _BotName.Source.Database
{
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    public class DatabaseSettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "SQLite";
        public string DefaultDataProvider => "SQLite";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "Base",
                        ProviderName = "SQLite",
                        ConnectionString = @"Data Source=:memory:;Version=3;New=True;"
                    };
            }
        }
    }
}