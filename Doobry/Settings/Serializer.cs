using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Doobry.Features.QueryDeveloper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Doobry.Settings
{
    public static class Serializer
    {
        public static string Stringify(IConnectionCache connectionCache, IGeneralSettings generalSettings, LayoutStructure layoutStructure)
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

        private static JObject ToJson(IGeneralSettings generalSettings)
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
            ts.selectedTabItemId = tabSet.SelectedTabItemId;
            ts.tabItems = new JArray(tabSet.TabItems.Select(ToJson));            
            return ts;
        }

        private static JObject ToJson(LayoutStructureTabItem tabItem)
        {
            if (tabItem == null) throw new ArgumentNullException(nameof(tabItem));

            dynamic ti = new JObject();
            ti.id = tabItem.Id;
            ti.featureId = tabItem.FeatureId;
            tabItem.BackingStoreWriter.WriteToBackingStore(tabItem.TabContentViewModel, ti);            
            return ti;
        }

        public static SettingsContainer Objectify(string data)
        {
            dynamic jObj = JObject.Parse(data);            
            JArray connectionsJArray = jObj.connections;
            var connections = connectionsJArray.Select(jt =>
                new Connection(
                    Guid.Parse(jt["id"].ToString()),
                    jt["label"].ToString(),
                    jt["host"].ToString(),
                    jt["authorisationKey"].ToString(),
                    jt["databaseId"].ToString(),
                    jt["collectionId"].ToString()));
            var connectionCache = new ConnectionCache(connections);            

            var generalSettings = new GeneralSettings((int?)jObj.general.maxItemCount.Value);

            var layoutStructure = ObjectifyLayout(jObj.layout);

            return new SettingsContainer(connectionCache, generalSettings, layoutStructure);
        }

        private static LayoutStructure ObjectifyLayout(dynamic layout)
        {
            JArray windowsJArray = layout.windows;
            var layoutStructureWindows = windowsJArray.Select(ObjectifyWindow);

            return new LayoutStructure(layoutStructureWindows);
        }

        private static LayoutStructureWindow ObjectifyWindow(JToken windowJToken)
        {
            return new LayoutStructureWindow(
                ((JArray)windowJToken.SelectToken("branches")).Select(ObjectifyBranch),
                ((JArray)windowJToken.SelectToken("tabSets")).Select(ObjectifyTabSet));            
        }

        private static LayoutStructureBranch ObjectifyBranch(JToken branchJToken)
        {
            return new LayoutStructureBranch(
                Guid.Parse(branchJToken["id"].ToString()),                
                GetNullableGuid(branchJToken, "childFirstBranchId"),
                GetNullableGuid(branchJToken, "childSecondBranchId"),
                GetNullableGuid(branchJToken, "childFirstTabSetId"),
                GetNullableGuid(branchJToken, "childSecondTabSetId"),
                branchJToken["orientaion"].ToObject<Orientation>(),
                branchJToken["ratio"].ToObject<double>());
        }

        private static LayoutStructureTabSet ObjectifyTabSet(JToken tabSetJToken)
        {
            return new LayoutStructureTabSet(
                Guid.Parse(tabSetJToken["id"].ToString()),
                GetNullableGuid(tabSetJToken, "selectedTabItemId"),
                ((JArray)tabSetJToken.SelectToken("tabItems")).Select(ObjectifyTabItem));
        }

        private static LayoutStructureTabItem ObjectifyTabItem(JToken tabItemJToken)
        {
            return new LayoutStructureTabItem(
                Guid.Parse(tabItemJToken["id"].ToString()),
                GetNullableGuid(tabItemJToken, "featureId") ?? QueryDeveloperFeatureFactory.MyFeatureId, //backwards compat with older layout files
                tabItemJToken);
        }

        private static Guid? GetNullableGuid(JToken token, string path)
        {
            Guid guid;
            return Guid.TryParse(token.SelectToken(path)?.ToString() ?? "", out guid)
                ? guid
                : (Guid?)null;
        }
    }
}
