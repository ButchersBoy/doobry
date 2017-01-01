namespace Doobry.Features
{
    public interface ITabContentLifetimeHost
    {
        object ViewModel { get; }

        void Cleanup(TabCloseReason closeReason);        
    }
}