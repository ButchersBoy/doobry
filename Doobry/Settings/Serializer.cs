using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Doobry.Settings
{
    public static class Serializer
    {
        public static string Stringify(Connection connection, GeneralSettings generalSettings)
        {
            //see http://www.newtonsoft.com/json/help/html/CreateJsonDynamic.htm

            dynamic settings = new JObject();
            settings.Connection = ToJson(connection);
            settings.General = ToJson(generalSettings);
            return settings.ToString();
        }

        private static JObject ToJson(Connection connection)
        {
            dynamic cn = new JObject();
            cn.Host = connection.Host;
            cn.AuthorisationKey = connection.AuthorisationKey;
            cn.DatabaseId = connection.DatabaseId;
            cn.CollectionId = connection.CollectionId;
            return cn;
        }

        private static JObject ToJson(GeneralSettings generalSettings)
        {
            dynamic gs = new JObject();
            gs.MaxItemCount = generalSettings.MaxItemCount;
            return gs;
        }

        //this signature will def' change
        public static Tuple<Connection, GeneralSettings> Objectify(string data)
        {
            dynamic jObj = JObject.Parse(data);
            var connection = new Connection(
                jObj.Connection.Host.Value, 
                jObj.Connection.AuthorisationKey.Value, 
                jObj.Connection.DatabaseId.Value,
                jObj.Connection.CollectionId.Value);
            var generalSettings = new GeneralSettings((int?)jObj.General.MaxItemCount.Value);

            return new Tuple<Connection, GeneralSettings>(connection, generalSettings);
        }
    }
}
