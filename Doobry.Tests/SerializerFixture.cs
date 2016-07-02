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
            var connection = new Connection("AAA", "BBB", "CCC", "DDD");
            var generalSettings = new GeneralSettings(5);

            var stringify = Serializer.Stringify(connection, generalSettings);

            stringify.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void WillRoundTrip()
        {
            var connection = new Connection("AAA", "BBB", "CCC", "DDD");
            var generalSettings = new GeneralSettings(5);
            var data = Serializer.Stringify(connection, generalSettings);

            var tuple = Serializer.Objectify(data);

            tuple.Item1.AuthorisationKey.ShouldBe(connection.AuthorisationKey);
            tuple.Item2.MaxItemCount.ShouldBe(generalSettings.MaxItemCount);
        }

        [Fact]
        public void WillRoundTripNull()
        {
            var connection = new Connection("AAA", "BBB", "CCC", "DDD");
            var generalSettings = new GeneralSettings(null);
            var data = Serializer.Stringify(connection, generalSettings);

            var tuple = Serializer.Objectify(data);
            
            tuple.Item2.MaxItemCount.ShouldBeNull();
        }
    }
}
