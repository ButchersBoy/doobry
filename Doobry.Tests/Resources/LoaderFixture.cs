using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doobry.Resources;
using Shouldly;
using Xunit;

namespace Doobry.Tests.Resources
{
    public class LoaderFixture
    {
        [Fact]
        public void WillLoadDocumentDbHighlighting()
        {
            var documentDbSyntaxHighlighting = new Loader().GetDocumentDbSyntaxHighlighting();

            documentDbSyntaxHighlighting.ShouldNotBeNull();
        }
    }
}
