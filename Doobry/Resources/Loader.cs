using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

namespace Doobry.Resources
{
    public class Loader
    {
        public IHighlightingDefinition GetDocumentDbSyntaxHighlighting()
        {
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(this.GetType(), "DocumentDb.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    return ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
    }
}
