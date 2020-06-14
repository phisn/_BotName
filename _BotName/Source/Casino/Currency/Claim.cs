﻿using System;
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
        private readonly Dictionary<ulong, DateTime> _claims = new Dictionary<ulong, DateTime>();
        private const int MinClaim = 1;
        private const int MaxClaim = 100;

        public void ResetAllClaims()
        {
            _claims.Clear();
        }
        
        public ClaimResult ClaimMoney(ulong userId, bool ignoreCooldown = false)
        {
            var result = new ClaimResult();
            var nextClaimTime = GetNextClaimTime(userId);
            if (nextClaimTime > DateTime.Now && !ignoreCooldown)
            {
                result.Status = ClaimError.Cooldown;
                result.NextClaimTime = nextClaimTime;
                return result;
            }

            if (_claims.ContainsKey(userId)) {
                _claims[userId] = DateTime.Now;
            } else { 
                _claims.Add(userId, DateTime.Now);
            }

            var claimedMoney = new Random().Next(MinClaim, MaxClaim + 1);

            var userRepository = _casinoController.GetCasinoUserRepository();
            var user = userRepository.FindOrCreateById(userId);
            userRepository.AddMoney(user, claimedMoney);

            result.Status = ClaimError.Okay;
            result.ClaimedAmount = claimedMoney;
            result.NextClaimTime = CalculateNextClaimTime(_claims[userId]);
            return result;
        }

        private DateTime GetNextClaimTime(ulong userId)
        {
            return !_claims.TryGetValue(userId, out var lastClaim) ? DateTime.Now : CalculateNextClaimTime(lastClaim);
        }

        private DateTime CalculateNextClaimTime(DateTime lastClaim)
        {
            return lastClaim.AddHours(1);
        }
    }
}