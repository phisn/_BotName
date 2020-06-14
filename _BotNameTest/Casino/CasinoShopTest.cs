using _BotName.Source;
using _BotName.Source.Casino;
using _BotName.Source.Casino.Shop;
using Moq;
using Xunit;

namespace _BotNameTest.Casino
{
    public class ShopTest
    {
        [Fact]
        public void BuyRoleWithNotEnoughMoney()
        {
            var discordMock = new Mock<DiscordUtility>();
            discordMock.Setup(d => d.AddRole("Lucky", 123)).Returns(true);
            
            var casinoUserRepoMock = new Mock<CasinoUserRepository>();
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(123)).Returns(new CasinoUser { Money = 100 });
            
            var casinoMock = new Mock<CasinoController>();
            casinoMock.Setup(c => c.GetCasinoUserRepository()).Returns(casinoUserRepoMock.Object);
            casinoMock.Setup(c => c.Initialize());
            
            // Buy lucky role (price: 10000) when only having 100 money
            ShopOrder order = new ShopOrder(123, "Lucky");
            order.OverrideDiscordUtility(discordMock.Object);
            order.OverrideCasinoController(casinoMock.Object);
            ShopError orderResult = order.perform();
            Assert.Equal(ShopError.NotEnoughMoney, orderResult);
            discordMock.Verify(mock => mock.AddRole("Lucky", 123), Times.Never);
            //casinoMock.Verify(mock => mock.Save(), Times.Never);
        }
        
        [Fact]
        public void BuyRoleWithEnoughMoney()
        {
            var discordMock = new Mock<DiscordUtility>();
            discordMock.Setup(d => d.AddRole("Lucky", 123)).Returns(true);
            
            var casinoUserRepoMock = new Mock<CasinoUserRepository>();
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(123)).Returns(new CasinoUser { Money = 10000 });
            
            var casinoMock = new Mock<CasinoController>();
            casinoMock.Setup(c => c.GetCasinoUserRepository()).Returns(casinoUserRepoMock.Object);
            casinoMock.Setup(c => c.Initialize());
            
            // Buy lucky role (price: 10000) when only having 100 money
            ShopOrder order = new ShopOrder(123, "Lucky");
            order.OverrideDiscordUtility(discordMock.Object);
            order.OverrideCasinoController(casinoMock.Object);
            ShopError orderResult = order.perform();
            Assert.Equal(ShopError.Okay, orderResult);
            discordMock.Verify(mock => mock.AddRole("Lucky", 123), Times.Once);
            //casinoMock.Verify(mock => mock.Save(), Times.Once);
        }
        
        [Fact]
        public void BuyRoleWithMoreThanEnoughMoney()
        {
            var discordMock = new Mock<DiscordUtility>();
            discordMock.Setup(d => d.AddRole("Lucky", 123)).Returns(true);
            
            var casinoUserRepoMock = new Mock<CasinoUserRepository>();
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(123)).Returns(new CasinoUser { Money = 10001 });
            
            var casinoMock = new Mock<CasinoController>();
            casinoMock.Setup(c => c.GetCasinoUserRepository()).Returns(casinoUserRepoMock.Object);
            casinoMock.Setup(c => c.Initialize());
            
            // Buy lucky role (price: 10000) when only having 100 money
            ShopOrder order = new ShopOrder(123, "Lucky");
            order.OverrideDiscordUtility(discordMock.Object);
            order.OverrideCasinoController(casinoMock.Object);
            ShopError orderResult = order.perform();
            Assert.Equal(ShopError.Okay, orderResult);
            discordMock.Verify(mock => mock.AddRole("Lucky", 123), Times.Once);
            //casinoMock.Verify(mock => mock.Save(), Times.Once);
        }
        
        [Fact]
        public void DiscordApiError()
        {
            var discordMock = new Mock<DiscordUtility>();
            discordMock.Setup(d => d.AddRole("Lucky", 123)).Returns(false);
            var casinoMock = new Mock<CasinoController>();
            
            var casinoUserRepoMock = new Mock<CasinoUserRepository>();
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(123)).Returns(new CasinoUser { Money = 10000 });

            casinoMock.Setup(c => c.GetCasinoUserRepository()).Returns(casinoUserRepoMock.Object);
            casinoMock.Setup(c => c.Initialize());
            
            // Buy lucky role (price: 10000) when only having 100 money
            ShopOrder order = new ShopOrder(123, "Lucky");
            order.OverrideDiscordUtility(discordMock.Object);
            order.OverrideCasinoController(casinoMock.Object);
            ShopError orderResult = order.perform();
            Assert.Equal(ShopError.RoleAwardError, orderResult);
            discordMock.Verify(mock => mock.AddRole("Lucky", 123), Times.Once);
            //casinoMock.Verify(mock => mock.Save(), Times.Never);
        }
        
        [Fact]
        public void BuyUnknownRole()
        {
            var discordMock = new Mock<DiscordUtility>();
            var casinoMock = new Mock<CasinoController>();
            
            var casinoUserRepoMock = new Mock<CasinoUserRepository>();
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(123)).Returns(new CasinoUser { Money = 100 });
            
            casinoMock.Setup(c => c.GetCasinoUserRepository()).Returns(casinoUserRepoMock.Object);
            casinoMock.Setup(c => c.Initialize());
            
            // Buy lucky role (price: 10000) when only having 100 money
            ShopOrder order = new ShopOrder(123, "UnknownRole");
            order.OverrideDiscordUtility(discordMock.Object);
            order.OverrideCasinoController(casinoMock.Object);
            ShopError orderResult = order.perform();
            Assert.Equal(ShopError.UnknownItem, orderResult);
            discordMock.Verify(mock => mock.AddRole(It.IsAny<string>(), 123), Times.Never);
            //casinoMock.Verify(mock => mock.Save(), Times.Never);
        }
    }
}