namespace Doobry.Settings
{
    public class InitialLayoutStructureProvider : IInitialLayoutStructureProvider
    {
        private LayoutStructure _layoutStructure;

        public InitialLayoutStructureProvider() : this(null)
        { }

        public InitialLayoutStructureProvider(LayoutStructure layoutStructure)
        {
            _layoutStructure = layoutStructure;
        }

        public bool TryTake(out LayoutStructure layoutStructure)
        {
            layoutStructure = _layoutStructure;
            _layoutStructure = null;
            return layoutStructure != null;
        }
    }
}