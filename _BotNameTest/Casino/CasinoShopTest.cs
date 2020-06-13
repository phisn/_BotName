using _BotName.Source;
using _BotName.Source.Casino;
using _BotName.Source.Casino.Shop;
using Moq;
using NUnit.Framework;

namespace _BotNameTest.Casino
{
    [TestFixture]
    public class ShopTest
    {
        [Test]
        public void BuyRoleWithNotEnoughMoney()
        {
            var discordMock = new Mock<DiscordUtility>(true);
            discordMock.Setup(d => d.AddRole("Lucky", 123)).Returns(true);
            var casinoMock = new Mock<CasinoController>(true);
            casinoMock.Setup(c => c.GetUser(123)).Returns(new CasinoUser { Money = 100 });
            casinoMock.Setup(c => c.Initialize());
            
            // Buy lucky role (price: 10000) when only having 100 money
            ShopOrder order = new ShopOrder(123, "Lucky", discordMock.Object, casinoMock.Object);
            ShopError orderResult = order.perform();
            Assert.AreEqual(ShopError.NotEnoughMoney, orderResult);
            discordMock.Verify(mock => mock.AddRole("Lucky", 123), Times.Never);
            casinoMock.Verify(mock => mock.Save(), Times.Never);
        }
        
        [Test]
        public void BuyRoleWithEnoughMoney()
        {
            var discordMock = new Mock<DiscordUtility>(true);
            discordMock.Setup(d => d.AddRole("Lucky", 123)).Returns(true);
            var casinoMock = new Mock<CasinoController>(true);
            casinoMock.Setup(c => c.GetUser(123)).Returns(new CasinoUser { Money = 10000 });
            casinoMock.Setup(c => c.Initialize());
            
            // Buy lucky role (price: 10000) when only having 100 money
            ShopOrder order = new ShopOrder(123, "Lucky", discordMock.Object, casinoMock.Object);
            ShopError orderResult = order.perform();
            Assert.AreEqual(ShopError.Okay, orderResult);
            discordMock.Verify(mock => mock.AddRole("Lucky", 123), Times.Once);
            casinoMock.Verify(mock => mock.Save(), Times.Once);
        }
        
        [Test]
        public void BuyRoleWithMoreThanEnoughMoney()
        {
            var discordMock = new Mock<DiscordUtility>(true);
            discordMock.Setup(d => d.AddRole("Lucky", 123)).Returns(true);
            var casinoMock = new Mock<CasinoController>(true);
            casinoMock.Setup(c => c.GetUser(123)).Returns(new CasinoUser { Money = 10001 });
            casinoMock.Setup(c => c.Initialize());
            
            // Buy lucky role (price: 10000) when only having 100 money
            ShopOrder order = new ShopOrder(123, "Lucky", discordMock.Object, casinoMock.Object);
            ShopError orderResult = order.perform();
            Assert.AreEqual(ShopError.Okay, orderResult);
            discordMock.Verify(mock => mock.AddRole("Lucky", 123), Times.Once);
            casinoMock.Verify(mock => mock.Save(), Times.Once);
        }
        
        [Test]
        public void DiscordApiError()
        {
            var discordMock = new Mock<DiscordUtility>(true);
            discordMock.Setup(d => d.AddRole("Lucky", 123)).Returns(false);
            var casinoMock = new Mock<CasinoController>(true);
            casinoMock.Setup(c => c.GetUser(123)).Returns(new CasinoUser { Money = 10000 });
            casinoMock.Setup(c => c.Initialize());
            
            // Buy lucky role (price: 10000) when only having 100 money
            ShopOrder order = new ShopOrder(123, "Lucky", discordMock.Object, casinoMock.Object);
            ShopError orderResult = order.perform();
            Assert.AreEqual(ShopError.RoleAwardError, orderResult);
            discordMock.Verify(mock => mock.AddRole("Lucky", 123), Times.Once);
            casinoMock.Verify(mock => mock.Save(), Times.Never);
        }
        
        [Test]
        public void BuyUnknownRole()
        {
            var discordMock = new Mock<DiscordUtility>(true);
            var casinoMock = new Mock<CasinoController>(true);
            casinoMock.Setup(c => c.GetUser(123)).Returns(new CasinoUser { Money = 100 });
            casinoMock.Setup(c => c.Initialize());
            
            // Buy lucky role (price: 10000) when only having 100 money
            ShopOrder order = new ShopOrder(123, "UnknownRole", discordMock.Object, casinoMock.Object);
            ShopError orderResult = order.perform();
            Assert.AreEqual(ShopError.UnknownItem, orderResult);
            discordMock.Verify(mock => mock.AddRole(It.IsAny<string>(), 123), Times.Never);
            casinoMock.Verify(mock => mock.Save(), Times.Never);
        }
    }
}