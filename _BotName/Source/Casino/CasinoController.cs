using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace _BotName.Source.Casino
{
    [Serializable]
    public class CasinoUser
    {
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
        public static CasinoController Instance { get { return lazy.Value; } }
        private static readonly Lazy<CasinoController> lazy =
            new Lazy<CasinoController>(() => new CasinoController(true));

        private static string filename = "casino.bin";
        private CasinoContainer casinoContainer;

        
        /**
         * Do not use this constructor directly. Please use .Instance property.
         */
        public CasinoController(bool usedFactory = false)
        {
            if (!usedFactory)
            {
                throw new Exception("Please access singleton instance by .Instance property!");
            }
            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();
        }

        public virtual void Initialize()
        {
            try
            {
                if (!File.Exists(filename))
                {
                    casinoContainer = new CasinoContainer();
                    Save();
                }
                else
                {
                    using (Stream stream = new FileStream(
                        filename,
                        FileMode.Open,
                        FileAccess.Read))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        casinoContainer = (CasinoContainer)formatter.Deserialize(stream);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Failed to initialize casino controller: {exception.Message}");
                throw;
            }
        }

        public virtual CasinoUser GetUser(ulong userID)
        {
            CasinoUser casinoUser;

            if (!casinoContainer.casinoUsers.TryGetValue(userID, out casinoUser))
            {
                casinoUser = new CasinoUser();
                casinoContainer.casinoUsers.Add(userID, casinoUser);
            }

            return casinoUser;
        }

        public virtual void Save()
        {
            try
            {
                using (Stream stream = new FileStream(
                    filename,
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, casinoContainer);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Failed to save casino container: {exception.Message}");
                throw;
            }
        }
    }
}
