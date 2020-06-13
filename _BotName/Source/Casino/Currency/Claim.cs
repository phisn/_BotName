﻿﻿using System;
using System.Collections.Generic;

namespace _BotName.Source.Casino.Currency
{
    public enum ClaimError
    {
        Okay = 1,
        Cooldown = 2
    }

    public class ClaimResult
    {
        public ClaimError Status;
        public int ClaimedAmount = -1;
        public DateTime NextClaimTime;
        public int NextClaimMinutes => (int) Math.Round((NextClaimTime - DateTime.Now).TotalMinutes);
    }
    
    public class Claim: AbstractCasinoUtility
    {
        private Dictionary<ulong, DateTime> claims = new Dictionary<ulong, DateTime>();
        private readonly int minClaim = 1;
        private readonly int maxClaim = 100;
        
        public Claim(CasinoController casinoController = null) : base(casinoController) { }
        
        public void ResetAllClaims()
        {
            claims.Clear();
        }
        
        public ClaimResult ClaimMoney(ulong userId, bool ignoreCooldown = false)
        {
            ClaimResult result = new ClaimResult();
            DateTime nextClaimTime = GetNextClaimTime(userId);
            if (nextClaimTime > DateTime.Now && !ignoreCooldown)
            {
                result.Status = ClaimError.Cooldown;
                result.NextClaimTime = nextClaimTime;
                return result;
            }

            if (claims.ContainsKey(userId)) {
                claims[userId] = DateTime.Now;
            } else { 
                claims.Add(userId, DateTime.Now);
            }

            var claimedMoney = new Random().Next(minClaim, maxClaim + 1);

            _casinoController.GetCasinoUserRepository().FindOrCreateById(userId).Money += claimedMoney;
            _casinoController.Save();

            result.Status = ClaimError.Okay;
            result.ClaimedAmount = claimedMoney;
            result.NextClaimTime = calculateNextClaimTime(claims[userId]);
            return result;
        }

        public DateTime GetNextClaimTime(ulong userId)
        {
            return !claims.TryGetValue(userId, out var lastClaim) ? DateTime.Now : calculateNextClaimTime(lastClaim);
        }

        protected DateTime calculateNextClaimTime(DateTime lastClaim)
        {
            return lastClaim.AddHours(1);
        }
    }
}