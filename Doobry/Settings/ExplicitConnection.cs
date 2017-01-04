using System;

namespace Doobry.Settings
{
    public class ExplicitConnection : Connection
    {
        public ExplicitConnection(Guid id, string label, string host, string authorisationKey, string databaseId, string collectionId)
            : base(host, authorisationKey, databaseId, collectionId)
        {
            Id = id;
            Label = label;
        }

        public Guid Id { get; }

        public string Label
        {
            get;
        }

        protected bool Equals(ExplicitConnection other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExplicitConnection) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}