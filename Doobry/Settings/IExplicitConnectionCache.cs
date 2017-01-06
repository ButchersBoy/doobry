using System;
using System.Collections.Generic;
using DynamicData;
using DynamicData.Kernel;

namespace Doobry.Settings
{
    public interface IExplicitConnectionCache : IEnumerable<ExplicitConnection>
    {
        Optional<ExplicitConnection> Get(Guid id);

        IObservable<IChangeSet<ExplicitConnection, Guid>> Connect();

        void AddOrUpdate(ExplicitConnection explicitConnection);

        void Delete(Guid id);
    }    
}