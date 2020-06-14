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
        public GiveResult GiveMoney(ulong senderUserId, ulong receiverUserId, int amount)
        {
            var result = new GiveResult { Amount = amount, SenderUid = senderUserId, ReceiverUid = receiverUserId };
            if (amount <= 0) {
                result.Status = GiveError.AmountLessThanZero;
                return result;
            }
            
            var userRepository = _casinoController.GetCasinoUserRepository();
            CasinoUser casinoUserGiver = userRepository.FindOrCreateById(senderUserId);
            if (casinoUserGiver.Money < amount)
            {
                result.Status = GiveError.NotEnoughMoney;
                return result;
            }
            
            CasinoUser casinoUserReceiver = userRepository.FindOrCreateById(receiverUserId);

            userRepository.SubtractMoney(casinoUserGiver, amount);
            userRepository.AddMoney(casinoUserReceiver, amount);

            result.Status = GiveError.Okay;
            return result;
        }
    }
}