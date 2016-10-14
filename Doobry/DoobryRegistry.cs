using Doobry.Infrastructure;
using Doobry.Resources;
using Doobry.Settings;
using ICSharpCode.AvalonEdit.Highlighting;
using StructureMap;

namespace Doobry
{
    public class DoobryRegistry : Registry
    {
        public DoobryRegistry()
        {            
            ForSingletonOf<IHighlightingDefinition>().Use(new Loader().GetDocumentDbSyntaxHighlighting());
            ForSingletonOf<IManualSaver>().Use<ManualSaver>();
            ForSingletonOf<ITabInstanceManager>().Use<TabInstanceManager>();
        }
    }
}