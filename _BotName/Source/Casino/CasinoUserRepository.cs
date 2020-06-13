using System;
using System.Collections.Generic;
using _BotName.Source.Core;

namespace _BotName.Source.Casino
{
    public class UserNotExistsException : Exception
    {
    }
    
    [Serializable]
    public class CasinoUserRepository: AbstractVolatileRepository<CasinoUser>
    {
        public CasinoUserRepository()
        {
            Data = new Dictionary<ulong, CasinoUser>();
        }
    }
}