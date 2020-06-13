namespace _BotName.Source.Casino
{
    public abstract class AbstractCasinoUtility
    {
        protected readonly CasinoController _casinoController;
        
        protected AbstractCasinoUtility(CasinoController casinoController = null) {
            _casinoController = casinoController ?? CasinoController.Instance;
        }
    }
}