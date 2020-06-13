namespace _BotName.Source.Casino.Currency
{
    public enum GiveError
    {
        Okay = 1,
        AmountLessThanZero = 2,
        NotEnoughMoney = 3
    }

    public class GiveResult
    {
        public GiveError Status;
        public ulong SenderUid;
        public ulong ReceiverUid;
        public int Amount;
    }

    public class Give: AbstractCasinoUtility
    {
        public Give(CasinoController casinoController = null) : base(casinoController) { }
        
        public GiveResult GiveMoney(ulong senderUserId, ulong receiverUserId, int amount)
        {
            var result = new GiveResult { Amount = amount, SenderUid = senderUserId, ReceiverUid = receiverUserId };
            if (amount <= 0) {
                result.Status = GiveError.AmountLessThanZero;
                return result;
            }
            
            CasinoUser casinoUserGiver = _casinoController.GetCasinoUserRepository().FindOrCreateById(senderUserId);
            if (casinoUserGiver.Money < amount)
            {
                result.Status = GiveError.NotEnoughMoney;
                return result;
            }
            
            CasinoUser casinoUserGetter = _casinoController.GetCasinoUserRepository().FindOrCreateById(receiverUserId);

            casinoUserGiver.SubtractMoney(amount);
            casinoUserGetter.AddMoney(amount);
            _casinoController.Save();

            result.Status = GiveError.Okay;
            return result;
        }
    }
}