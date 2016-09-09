using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doobry.Settings;
using Shouldly;
using Xunit;

namespace Doobry.Tests
{
    public class SerializerFixture
    {
        [Fact]
        public void MakesJson()
        {
            var connection1 = new Connection(Guid.NewGuid(), "AAA", "BBB", "CCC", "DDD", "EEE");
            var connection2 = new Connection(Guid.NewGuid(), "111", "222", "333", "444", "555");
            var generalSettings = new GeneralSettings(5);
            var connectionCache = new ConnectionCache(new[] {connection1, connection2});

            var stringify = Serializer.Stringify(connectionCache, generalSettings, new LayoutStructure(Enumerable.Empty<LayoutStructureWindow>()));

            stringify.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void WillRoundTrip()
        {
            var connection1Id = Guid.NewGuid();
            var connection2Id = Guid.NewGuid();
            var connection1 = new Connection(connection1Id, "AAA", "BBB", "CCC", "DDD", "EEE");
            var connection2 = new Connection(connection2Id, "111", "222", "333", "444", "555");
            var generalSettings = new GeneralSettings(5);
            var connectionCache = new ConnectionCache(new[] { connection1, connection2 });

            var data = Serializer.Stringify(connectionCache, generalSettings, new LayoutStructure(Enumerable.Empty<LayoutStructureWindow>()));
            var settingsContainer = Serializer.Objectify(data);

            settingsContainer.GeneralSettings.MaxItemCount.ShouldBe(generalSettings.MaxItemCount);
            settingsContainer.ConnectionCache.Get(connection1Id).Id.ShouldBe(connection1Id);
            settingsContainer.ConnectionCache.Get(connection1Id).AuthorisationKey.ShouldBe(connection1.AuthorisationKey);
            settingsContainer.ConnectionCache.Get(connection1Id).CollectionId.ShouldBe(connection1.CollectionId);
            settingsContainer.ConnectionCache.Get(connection1Id).DatabaseId.ShouldBe(connection1.DatabaseId);
            settingsContainer.ConnectionCache.Get(connection1Id).Host.ShouldBe(connection1.Host);
            settingsContainer.ConnectionCache.Get(connection1Id).Label.ShouldBe(connection1.Label);
            settingsContainer.ConnectionCache.Get(connection2Id).Id.ShouldBe(connection2Id);
            settingsContainer.ConnectionCache.Get(connection2Id).AuthorisationKey.ShouldBe(connection2.AuthorisationKey);
            settingsContainer.ConnectionCache.Get(connection2Id).CollectionId.ShouldBe(connection2.CollectionId);
            settingsContainer.ConnectionCache.Get(connection2Id).DatabaseId.ShouldBe(connection2.DatabaseId);
            settingsContainer.ConnectionCache.Get(connection2Id).Host.ShouldBe(connection2.Host);
            settingsContainer.ConnectionCache.Get(connection2Id).Label.ShouldBe(connection2.Label);

            settingsContainer.GeneralSettings.MaxItemCount.ShouldBe(generalSettings.MaxItemCount);
        }

        [Fact]
        public void WillRoundTripNull()
        {
            var generalSettings = new GeneralSettings(null);
            var data = Serializer.Stringify(new ConnectionCache(), generalSettings, new LayoutStructure(Enumerable.Empty<LayoutStructureWindow>()));

            var settingsContainer = Serializer.Objectify(data);

            settingsContainer.GeneralSettings.MaxItemCount.ShouldBeNull();
        }
    }
}
