using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Doobry.Settings;
using DynamicData;
using DynamicData.Kernel;

namespace Doobry.Features.Management
{
    internal static class LocalExtensions
    {
        public static IEnumerable<TConnection> GetOptionalConnections<TConnection, TKey>(
            this Optional<IGroup<TConnection, TKey, GroupedConnectionKey>> value) where TConnection : Connection
        {
            return value.HasValue
                ? value.Value.Cache.Items
                : Enumerable.Empty<TConnection>();
        }

        public static IDisposable BuildChildNodes<TNode>(
            this IExplicitConnectionCache explicitConnectionCache,
            IImplicitConnectionCache implicitConnectionCache,
            GroupedConnectionKey parentKey,
            GroupedConnectionKeyLevel childKeyLevel,
            Func<GroupedConnection, TNode> childNodeFactory,
            IComparer<TNode> sortComparer, 
            IScheduler scheduler, 
            out ReadOnlyObservableCollection<TNode> nodes)
        {
            return explicitConnectionCache.Connect()
                //user could dupe the connections, so we group em all
                .Filter(FilterBuilder<ExplicitConnection>(parentKey))
                .Group(explicitCn => new GroupedConnectionKey(explicitCn, childKeyLevel))
                .FullJoin(
                    implicitConnectionCache.Connect()
                        .Filter(FilterBuilder<ImplicitConnection>(parentKey))
                        .Group(
                            implicitCn =>
                                new GroupedConnectionKey(implicitCn, childKeyLevel)),
                    implicitGroup => implicitGroup.Key,
                    (key, left, right) =>
                        new GroupedConnection(key, left.GetOptionalConnections(), right.GetOptionalConnections()))
                .Filter(gc => !string.IsNullOrEmpty(gc[childKeyLevel]))
                .Transform(childNodeFactory)
                //.Sort(sortComparer)
                .DisposeMany()
                .ObserveOn(scheduler)
                .Bind(out nodes)
                .Subscribe();
        }

        private static Func<TConnection, bool> FilterBuilder<TConnection>(GroupedConnectionKey parentKey) 
            where TConnection : IConnection
        {
            return cn =>
                parentKey == null
                ||
                Equals(new GroupedConnectionKey(cn, parentKey.Level), parentKey);
        }
    }
}