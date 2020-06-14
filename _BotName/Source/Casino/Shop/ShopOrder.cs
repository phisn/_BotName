namespace _BotName.Source.Casino.Shop
{
   public class ShopOrder: AbstractCasinoUtility
   {
        private DiscordUtility _discordUtility;
        private readonly ulong _userId;
        private readonly string _roleName;
        
        public ShopOrder(ulong userId, string roleName) {
            _roleName = roleName;
            _userId = userId;
            _discordUtility = DiscordUtility.Instance;
        }

        public void OverrideDiscordUtility(DiscordUtility discordUtility)
        {
            _discordUtility = discordUtility;
        }

        public ShopError perform()
        {
            int price = GetPrice();
            if (price < 0) {
                return ShopError.UnknownItem;
            }
            CasinoUser casinoUser = GetCasinoUser();
            if (casinoUser.Money < price) {
                return ShopError.NotEnoughMoney;
            }
            
            // Award discord role
            if (!_discordUtility.AddRole(_roleName, _userId))
            {
                return ShopError.RoleAwardError;
            }
            
            casinoUser.Money -= price;
            // _casinoController.Save();
            return ShopError.Okay;
        }

        protected CasinoUser GetCasinoUser()
        {
            return _casinoController.GetCasinoUserRepository().FindOrCreateById(_userId);
        }

        public int GetPrice()
        {
            switch (_roleName.ToLower())
            {
                case "lucky":
                    return 10000;
                default:
                    return -1;
            }
        }
    }
}