using _BotName.Source.Casino;
using _BotName.Source.Casino.Currency;
using Moq;
using Xunit;

namespace _BotNameTest.Casino.Currency
{
    public class GiveTest
    {
        [Theory]
        [InlineData(100, 5, 100, GiveError.Okay, 0, 105)]
        [InlineData(100, 5, 1000, GiveError.NotEnoughMoney, 100, 5)]
        [InlineData(100, 5, -5, GiveError.AmountLessThanZero, 100, 5)]
        public void GiveMoneyToAnotherUser(int moneySender, int moneyReceiver, int amount, GiveError expectedStatus, int moneySenderAfter, int moneyReceiverAfter)
        {
            var casinoMock = new Mock<CasinoController>(true);
            var casinoUserRepoMock = new Mock<CasinoUserRepository>();

            var userSender = new CasinoUser {Money = moneySender};
            var userReceiver = new CasinoUser {Money = moneyReceiver};
            
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(1)).Returns(userSender);
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(2)).Returns(userReceiver);
            casinoMock.Setup(c => c.GetCasinoUserRepository()).Returns(casinoUserRepoMock.Object);
            casinoMock.Setup(c => c.Initialize());
            
            var give = new Give(casinoMock.Object);
            
            var giveResult = give.GiveMoney(1, 2, amount);
            Assert.Equal(expectedStatus, giveResult.Status);
            Assert.Equal((ulong)1, giveResult.SenderUid);
            Assert.Equal((ulong)2, giveResult.ReceiverUid);
            Assert.Equal(amount, giveResult.Amount);
            Assert.Equal(moneySenderAfter, userSender.Money);
            Assert.Equal(moneyReceiverAfter, userReceiver.Money);
        }
    }
}