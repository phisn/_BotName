using _BotName.Source.Casino;
using _BotName.Source.Casino.Currency;
using Moq;
using Xunit;

namespace _BotNameTest.Casino.Currency
{
    public class GiveTest
    {
        [Theory]
        [InlineData(100, 5, 100, GiveError.Okay)]
        [InlineData(100, 5, 1000, GiveError.NotEnoughMoney)]
        [InlineData(100, 5, -5, GiveError.AmountLessThanZero)]
        public void GiveMoneyToAnotherUser(int moneySender, int moneyReceiver, int amount, GiveError expectedStatus)
        {
            var casinoMock = new Mock<CasinoController>();
            var casinoUserRepoMock = new Mock<CasinoUserRepository>();

            var userSender = new CasinoUser {Money = moneySender};
            var userReceiver = new CasinoUser {Money = moneyReceiver};
            
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(1)).Returns(userSender);
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(2)).Returns(userReceiver);
            casinoMock.Setup(c => c.GetCasinoUserRepository()).Returns(casinoUserRepoMock.Object);
            casinoMock.Setup(c => c.Initialize());
            
            var give = new Give();
            give.OverrideCasinoController(casinoMock.Object);
            
            var giveResult = give.GiveMoney(1, 2, amount);
            Assert.Equal(expectedStatus, giveResult.Status);
            Assert.Equal((ulong)1, giveResult.SenderUid);
            Assert.Equal((ulong)2, giveResult.ReceiverUid);
            Assert.Equal(amount, giveResult.Amount);
            var timesObj = Times.Never();
            if (expectedStatus == GiveError.Okay) {
                timesObj = Times.Once();
            }
            casinoUserRepoMock.Verify(mock => mock.SubtractMoney(userSender, amount), timesObj);
            casinoUserRepoMock.Verify(mock => mock.AddMoney(userReceiver, amount), timesObj);
        }
    }
}