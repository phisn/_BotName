using _BotName.Source.Casino;
using LinqToDB;

namespace _BotName.Source.Database
{
    public class DatabaseManager : LinqToDB.Data.DataConnection
    {
        public DatabaseManager() : base("Main") { }

        public ITable<CasinoUser> CasinoUser => GetTable<CasinoUser>();
    }
}