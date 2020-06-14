using System;
using _BotName.Source.Casino;
using _BotName.Source.Casino.Currency;
using Moq;
using Xunit;

namespace _BotNameTest.Casino.Currency
{
    public class ClaimTest
    {
        [Fact]
        public void ClaimAndTestCooldown()
        {
            var casinoMock = new Mock<CasinoController>();
            var casinoUserRepoMock = new Mock<CasinoUserRepository>();
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(123)).Returns(new CasinoUser { Money = 100 });
            casinoMock.Setup(c => c.GetCasinoUserRepository()).Returns(casinoUserRepoMock.Object);
            casinoMock.Setup(c => c.Initialize());
            
            var order = new Claim();
            order.OverrideCasinoController(casinoMock.Object);
            
            // First claim succeeds
            var claimResult = order.ClaimMoney(123);
            Assert.Equal(ClaimError.Okay, claimResult.Status);
            Assert.NotEqual(0, claimResult.ClaimedAmount);
            //casinoMock.Verify(mock => mock.Save(), Times.Once);
            casinoMock.Invocations.Clear();
            
            // Second claim fails due to cooldown
            claimResult = order.ClaimMoney(123);
            Assert.Equal(ClaimError.Cooldown, claimResult.Status);
            Assert.Equal(-1, claimResult.ClaimedAmount);
            Assert.True(claimResult.NextClaimTime > DateTime.Now);
            //casinoMock.Verify(mock => mock.Save(), Times.Never);
        }
        
        [Fact]
        public void ClaimAndForceClaim()
        {
            var casinoMock = new Mock<CasinoController>();
            var casinoUser = new CasinoUser {Money = 100};
            
            var casinoUserRepoMock = new Mock<CasinoUserRepository>();
            casinoUserRepoMock.Setup(c => c.FindOrCreateById(123)).Returns(casinoUser);
            casinoMock.Setup(c => c.GetCasinoUserRepository()).Returns(casinoUserRepoMock.Object);
            casinoMock.Setup(c => c.Initialize());
            
            var order = new Claim();
            order.OverrideCasinoController(casinoMock.Object);

            // First claim succeeds
            var claimResult = order.ClaimMoney(123);
            Assert.Equal(ClaimError.Okay, claimResult.Status);
            Assert.NotEqual(0, claimResult.ClaimedAmount);
            casinoUserRepoMock.Verify(mock => mock.AddMoney(casinoUser, claimResult.ClaimedAmount), Times.Once);
            casinoMock.Invocations.Clear();
            
            // Force claim succeeds
            claimResult = order.ClaimMoney(123, true);
            Assert.Equal(ClaimError.Okay, claimResult.Status);
            Assert.NotEqual(0, claimResult.ClaimedAmount);
            casinoUserRepoMock.Verify(mock => mock.AddMoney(casinoUser, claimResult.ClaimedAmount), Times.Once);
            casinoMock.Invocations.Clear();
            
            // Force claim should set the cooldown nonetheless
            claimResult = order.ClaimMoney(123);
            Assert.Equal(ClaimError.Cooldown, claimResult.Status);
            Assert.Equal(-1, claimResult.ClaimedAmount);
            Assert.True(claimResult.NextClaimTime > DateTime.Now);
            casinoUserRepoMock.Verify(mock => mock.AddMoney(casinoUser, claimResult.ClaimedAmount), Times.Never);
        }
    }
}