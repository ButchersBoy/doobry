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
            var connection1 = new ExplicitConnection(Guid.NewGuid(), "AAA", "BBB", "CCC", "DDD", "EEE");
            var connection2 = new ExplicitConnection(Guid.NewGuid(), "111", "222", "333", "444", "555");
            var generalSettings = new GeneralSettings(5);
            var connectionCache = new ExplicitConnectionCache(new[] {connection1, connection2});

            var stringify = Serializer.Stringify(connectionCache, generalSettings, new LayoutStructure(Enumerable.Empty<LayoutStructureWindow>()));

            stringify.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void WillRoundTrip()
        {
            var connection1Id = Guid.NewGuid();
            var connection2Id = Guid.NewGuid();
            var connection1 = new ExplicitConnection(connection1Id, "AAA", "BBB", "CCC", "DDD", "EEE");
            var connection2 = new ExplicitConnection(connection2Id, "111", "222", "333", "444", "555");
            var generalSettings = new GeneralSettings(5);
            var connectionCache = new ExplicitConnectionCache(new[] { connection1, connection2 });

            var data = Serializer.Stringify(connectionCache, generalSettings, new LayoutStructure(Enumerable.Empty<LayoutStructureWindow>()));
            var settingsContainer = Serializer.Objectify(data);

            settingsContainer.GeneralSettings.MaxItemCount.ShouldBe(generalSettings.MaxItemCount);
            settingsContainer.ExplicitConnectionCache.Get(connection1Id).Value.Id.ShouldBe(connection1Id);
            settingsContainer.ExplicitConnectionCache.Get(connection1Id).Value.AuthorisationKey.ShouldBe(connection1.AuthorisationKey);
            settingsContainer.ExplicitConnectionCache.Get(connection1Id).Value.CollectionId.ShouldBe(connection1.CollectionId);
            settingsContainer.ExplicitConnectionCache.Get(connection1Id).Value.DatabaseId.ShouldBe(connection1.DatabaseId);
            settingsContainer.ExplicitConnectionCache.Get(connection1Id).Value.Host.ShouldBe(connection1.Host);
            settingsContainer.ExplicitConnectionCache.Get(connection1Id).Value.Label.ShouldBe(connection1.Label);
            settingsContainer.ExplicitConnectionCache.Get(connection2Id).Value.Id.ShouldBe(connection2Id);
            settingsContainer.ExplicitConnectionCache.Get(connection2Id).Value.AuthorisationKey.ShouldBe(connection2.AuthorisationKey);
            settingsContainer.ExplicitConnectionCache.Get(connection2Id).Value.CollectionId.ShouldBe(connection2.CollectionId);
            settingsContainer.ExplicitConnectionCache.Get(connection2Id).Value.DatabaseId.ShouldBe(connection2.DatabaseId);
            settingsContainer.ExplicitConnectionCache.Get(connection2Id).Value.Host.ShouldBe(connection2.Host);
            settingsContainer.ExplicitConnectionCache.Get(connection2Id).Value.Label.ShouldBe(connection2.Label);

            settingsContainer.GeneralSettings.MaxItemCount.ShouldBe(generalSettings.MaxItemCount);
        }

        [Fact]
        public void WillRoundTripNull()
        {
            var generalSettings = new GeneralSettings(null);
            var data = Serializer.Stringify(new ExplicitConnectionCache(), generalSettings, new LayoutStructure(Enumerable.Empty<LayoutStructureWindow>()));

            var settingsContainer = Serializer.Objectify(data);

            settingsContainer.GeneralSettings.MaxItemCount.ShouldBeNull();
        }
    }
}
