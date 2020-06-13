using System;
using _BotName.Source.Casino;
using _BotName.Source.Casino.Currency;
using Moq;
using NUnit.Framework;

namespace _BotNameTest.Casino.Currency
{
    [TestFixture]
    public class ClaimTest
    {
        [Test]
        public void ClaimAndTestCooldown()
        {
            var casinoMock = new Mock<CasinoController>(true);
            casinoMock.Setup(c => c.GetUser(123)).Returns(new CasinoUser { Money = 100 });
            casinoMock.Setup(c => c.Initialize());
            
            var order = new Claim(casinoMock.Object);
            
            // First claim succeeds
            var claimResult = order.ClaimMoney(123);
            Assert.AreEqual(ClaimError.Okay, claimResult.Status);
            Assert.AreNotEqual(0, claimResult.ClaimedAmount);
            casinoMock.Verify(mock => mock.Save(), Times.Once);
            casinoMock.Invocations.Clear();
            
            // Second claim fails due to cooldown
            claimResult = order.ClaimMoney(123);
            Assert.AreEqual(ClaimError.Cooldown, claimResult.Status);
            Assert.AreEqual(-1, claimResult.ClaimedAmount);
            Assert.AreEqual(true, claimResult.NextClaimTime > DateTime.Now);
            casinoMock.Verify(mock => mock.Save(), Times.Never);
        }
        
        [Test]
        public void ClaimAndForceClaim()
        {
            var casinoMock = new Mock<CasinoController>(true);
            var casinoUser = new CasinoUser {Money = 100};
            casinoMock.Setup(c => c.GetUser(123)).Returns(casinoUser);
            casinoMock.Setup(c => c.Initialize());
            
            Claim order = new Claim(casinoMock.Object);

            // First claim succeeds
            var moneyBefore = casinoUser.Money;
            var claimResult = order.ClaimMoney(123);
            Assert.AreEqual(ClaimError.Okay, claimResult.Status);
            Assert.AreNotEqual(0, claimResult.ClaimedAmount);
            Assert.AreEqual(moneyBefore + claimResult.ClaimedAmount, casinoUser.Money);
            casinoMock.Verify(mock => mock.Save(), Times.Once);
            casinoMock.Invocations.Clear();
            
            // Force claim succeeds
            claimResult = order.ClaimMoney(123, true);
            Assert.AreEqual(ClaimError.Okay, claimResult.Status);
            Assert.AreNotEqual(0, claimResult.ClaimedAmount);
            casinoMock.Verify(mock => mock.Save(), Times.Once);
            casinoMock.Invocations.Clear();
            
            // Force claim should set the cooldown nonetheless
            claimResult = order.ClaimMoney(123);
            Assert.AreEqual(ClaimError.Cooldown, claimResult.Status);
            Assert.AreEqual(-1, claimResult.ClaimedAmount);
            Assert.AreEqual(true, claimResult.NextClaimTime > DateTime.Now);
            casinoMock.Verify(mock => mock.Save(), Times.Never);
        }
    }
}