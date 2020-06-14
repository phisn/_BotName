using System;

namespace _BotName.Source.Casino
{
    public class CasinoController
    {
        public static CasinoController Instance { get { return lazy.Value; } }
        private static readonly Lazy<CasinoController> lazy =
            new Lazy<CasinoController>(() => new CasinoController());

        private CasinoUserRepository _casinoUserRepository;

        protected CasinoController()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Initialize();
        }

        public virtual void Initialize()
        {
            _casinoUserRepository = new CasinoUserRepository();
        }

        public virtual CasinoUserRepository GetCasinoUserRepository()
        {
            return _casinoUserRepository;
        }
    }
}
