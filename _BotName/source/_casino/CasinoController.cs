using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.source._casino
{
    [Serializable]
    public class CasinoUser
    {
        public DateTime lastPlayTime = new DateTime();
        public int Money = 100;
    }

    [Serializable]
    public class CasinoContainer
    {
        public Dictionary<ulong, CasinoUser> casinoUsers;

        public CasinoContainer()
        {
            casinoUsers = new Dictionary<ulong, CasinoUser>();
        }
    }

    public class CasinoController
    {
        static private CasinoController instance;
        static public CasinoController Instance
        {
            get
            {
                if (instance == null)
                    instance = new CasinoController();

                return instance;
            }
        }

        private CasinoContainer casinoContainer = new CasinoContainer();

        public CasinoUser GetUser(ulong userID)
        {
            CasinoUser casinoUser;

            if (!casinoContainer.casinoUsers.TryGetValue(userID, out casinoUser))
            {
                casinoUser = new CasinoUser();
                casinoContainer.casinoUsers.Add(userID, casinoUser);
            }

            return casinoUser;
        }
    }
}
