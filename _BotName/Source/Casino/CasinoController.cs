using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace _BotName.Source.Casino
{
    [Serializable]
    public class CasinoUser
    {
        public int Money = 100;

        public void AddMoney(int amount)
        {
            Money += amount;
        }

        public void SubtractMoney(int amount)
        {
            Money -= amount;
        }
    }

    public class CasinoController
    {
        public static CasinoController Instance { get { return lazy.Value; } }
        private static readonly Lazy<CasinoController> lazy =
            new Lazy<CasinoController>(() => new CasinoController(true));

        private static string filename = "casino.bin";

        private CasinoUserRepository _casinoUserRepository;

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
                    _casinoUserRepository = new CasinoUserRepository();
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
                        _casinoUserRepository = (CasinoUserRepository)formatter.Deserialize(stream);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Failed to initialize casino controller: {exception.Message}");
                throw;
            }
        }

        public virtual CasinoUserRepository GetCasinoUserRepository()
        {
            return _casinoUserRepository;
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
                    formatter.Serialize(stream, _casinoUserRepository);
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
