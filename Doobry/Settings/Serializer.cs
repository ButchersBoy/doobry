using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Doobry.Settings
{
    public static class Serializer
    {
        public static string Stringify(ConnectionCache connectionCache, GeneralSettings generalSettings, LayoutStructure layoutStructure)
        {
            if (connectionCache == null) throw new ArgumentNullException(nameof(connectionCache));
            if (generalSettings == null) throw new ArgumentNullException(nameof(generalSettings));
            if (layoutStructure == null) throw new ArgumentNullException(nameof(layoutStructure));

            dynamic settings = new JObject();
            settings.connections = new JArray(connectionCache.Select(ToJson));
            settings.general = ToJson(generalSettings);
            settings.layout = ToJson(layoutStructure);
            return settings.ToString();
        }

        [Obsolete]
        public static string Stringify(Connection connection, GeneralSettings generalSettings)
        {
            //see http://www.newtonsoft.com/json/help/html/CreateJsonDynamic.htm

            dynamic settings = new JObject();
            settings.connection = ToJson(connection);
            settings.general = ToJson(generalSettings);
            return settings.ToString();
        }

        private static JObject ToJson(Connection connection)
        {
            dynamic cn = new JObject();
            cn.id = connection.Id;
            cn.label = connection.Label;
            cn.host = connection.Host;
            cn.authorisationKey = connection.AuthorisationKey;
            cn.databaseId = connection.DatabaseId;
            cn.collectionId = connection.CollectionId;
            return cn;
        }

        private static JObject ToJson(GeneralSettings generalSettings)
        {
            dynamic gs = new JObject();
            gs.maxItemCount = generalSettings.MaxItemCount;
            return gs;
        }

        private static JObject ToJson(LayoutStructure layoutStructure)
        {
            dynamic ls = new JObject();
            ls.windows = new JArray(layoutStructure.Windows.Select(ToJson));
            return ls;
        }

        private static JObject ToJson(LayoutStructureWindow window)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));

            dynamic w = new JObject();
            w.branches = new JArray(window.Branches.Select(ToJson));
            w.tabSets = new JArray(window.TabSets.Select(ToJson));
            return w;
        }

        private static JObject ToJson(LayoutStructureBranch branch)
        {
            if (branch == null) throw new ArgumentNullException(nameof(branch));

            dynamic b = new JObject();
            b.id = branch.Id;
            b.childFirstTabSetId = branch.ChildFirstTabSetId;
            b.childSecondTabSetId = branch.ChildSecondTabSetId;
            b.childFirstBranchId = branch.ChildFirstBranchId;
            b.childSecondBranchId = branch.ChildSecondBranchId;
            b.orientaion = branch.Orientation;
            b.ratio = branch.Ratio;
            return b;
        }

        private static JObject ToJson(LayoutStructureTabSet tabSet)
        {
            if (tabSet == null) throw new ArgumentNullException(nameof(tabSet));

            dynamic ts = new JObject();
            ts.id = tabSet.Id;
            ts.TabItems = new JArray(tabSet.TabItems.Select(ToJson));            
            return ts;
        }

        private static JObject ToJson(LayoutStructureTabItem tabItem)
        {
            if (tabItem == null) throw new ArgumentNullException(nameof(tabItem));

            dynamic ti = new JObject();
            ti.ConnectionId = tabItem.ConnectionId;
            return ti;
        }

        public static SettingsContainer Objectify(string data)
        {
            dynamic jObj = JObject.Parse(data);            
            JArray connectionsJArray = jObj.Connections;
            var connections = connectionsJArray.Select(jt =>
                new Connection(
                    Guid.Parse(jt["id"].ToString()),
                    jt["label"].ToString(),
                    jt["host"].ToString(),
                    jt["authorisationKey"].ToString(),
                    jt["databaseId"].ToString(),
                    jt["collectionId"].ToString()));
            var connectionCache = new ConnectionCache(connections);            

            var generalSettings = new GeneralSettings((int?)jObj.General.MaxItemCount.Value);

            return new SettingsContainer(connectionCache, generalSettings);
        }
    }
}
