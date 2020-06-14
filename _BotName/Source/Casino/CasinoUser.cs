using System.ComponentModel;
using LinqToDB.Mapping;

namespace _BotName.Source.Casino
{
    [Table(Name = "CasinoUser")]
    public class CasinoUser
    {
        [PrimaryKey, Identity]
        public ulong UserId { get; set; }
        
        [Column(Name = "Money"), NotNull, DefaultValue(100)]
        public int Money { get; set; }
    }
}