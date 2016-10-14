using System;

namespace Doobry
{
    public class DocumentChangedUnit
    {
        public DocumentChangedUnit(Guid tabId, string text)
        {
            TabId = tabId;
            Text = text;
        }

        public Guid TabId { get; }

        public string Text { get; }
    }
}