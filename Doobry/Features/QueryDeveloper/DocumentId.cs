using System;
using Newtonsoft.Json.Linq;

namespace Doobry.Features.QueryDeveloper
{
    public struct DocumentId : IEquatable<DocumentId>
    {
        public static DocumentId Empty => new DocumentId(null, null);

        public DocumentId(string raw, string self)
        {
            if (string.IsNullOrWhiteSpace(raw) ^ string.IsNullOrWhiteSpace(self))
                throw new ArgumentException("raw and self must be supplied together.", nameof(raw));

            Raw = raw ?? "";
            Self = self ?? "";
        }

        public static bool TryParse(string content, out DocumentId documentId)
        {
            try
            {
                var jObject = JObject.Parse(content);

                var idToken = jObject["id"];
                var selfToken = jObject["_self"];

                if (idToken != null && selfToken != null)
                {
                    var id = idToken.ToString();
                    var self = selfToken.ToString();

                    documentId = new DocumentId(id, self);
                    return true;
                }

                documentId = Empty;
                return false;
            }
            catch
            {
                documentId = Empty;
                return false;
            }
        }

        public string Raw { get; }

        public string Self { get; }

        public bool Equals(DocumentId other)
        {
            return string.Equals(Raw, other.Raw) && string.Equals(Self, other.Self);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DocumentId && Equals((DocumentId)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Raw?.GetHashCode() ?? 0) * 397) ^ (Self?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(DocumentId left, DocumentId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DocumentId left, DocumentId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return Raw;
        }
    }
}