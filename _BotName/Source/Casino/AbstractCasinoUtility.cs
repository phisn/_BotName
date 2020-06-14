namespace _BotName.Source.Casino
{
    public abstract class AbstractCasinoUtility
    {
        protected CasinoController _casinoController;
        
        protected AbstractCasinoUtility() {
            _casinoController = CasinoController.Instance;
        }

        public void OverrideCasinoController(CasinoController casinoController)
        {
            _casinoController = casinoController;
        }
    }
}