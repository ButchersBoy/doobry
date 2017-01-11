using System;

namespace Doobry.Features.QueryDeveloper
{
    public class DocumentChangedUnit
    {
        public DocumentChangedUnit(Guid fileId, string text)
        {
            FileId = fileId;
            Text = text;
        }

        public Guid FileId { get; }

        public string Text { get; }
    }
}