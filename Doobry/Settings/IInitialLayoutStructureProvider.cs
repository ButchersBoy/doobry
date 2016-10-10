namespace Doobry.Settings
{
    public interface IInitialLayoutStructureProvider
    {
        bool TryTake(out LayoutStructure layoutStructure);
    }
}