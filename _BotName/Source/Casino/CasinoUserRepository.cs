using System;
using System.Linq;
using _BotName.Source.Core;
using _BotName.Source.Database;
using LinqToDB;

namespace _BotName.Source.Casino
{
    public class CasinoUserRepository: AbstractRepository<CasinoUser>
    {
        public override CasinoUser FindById(ulong userId)
        {
            using (var db = new DatabaseManager())
            {
                var query = from p in db.CasinoUser
                    where p.UserId == userId
                    select p;
                return query.First();
            }
        }
        
        public override CasinoUser CreateForId(ulong userId)
        {
            using (var db = new DatabaseManager())
            {
                db.CasinoUser
                    .Value(p => p.UserId, userId)
                    .Insert();
                return FindById(userId);
            }
        }
        
        public virtual void AddMoney(CasinoUser user, int amount)
        {
            using (var db = new DatabaseManager())
            {
                db.CasinoUser
                    .Where(p => p.UserId == user.UserId)
                    .Set(p => p.Money, user.Money + amount)
                    .Update();
            }
        }
        
        public virtual void SubtractMoney(CasinoUser user, int amount)
        {
            using (var db = new DatabaseManager())
            {
                db.CasinoUser
                    .Where(p => p.UserId == user.UserId)
                    .Set(p => p.Money, user.Money - amount)
                    .Update();
            }
        }
    }
}