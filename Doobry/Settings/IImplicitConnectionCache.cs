using System;
using System.Collections.Generic;
using DynamicData;

namespace Doobry.Settings
{
    public interface IImplicitConnectionCache
    {
        IObservable<IChangeSet<ImplicitConnection, int>> Connect();

        void Merge(string source, IEnumerable<Connection> sourceConnections);
    }
}