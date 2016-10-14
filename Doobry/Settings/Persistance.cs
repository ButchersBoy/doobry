using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doobry.Settings
{
    public class Persistance
    {
        private const string ConfigurationFileName = "doobry-settings-auto.doobry";
        public static readonly string ConfigurationFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConfigurationFileName);

        public static readonly string QueryFileFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "doobry-docs");

        public bool TryLoadRaw(out string rawData)
        {
            if (File.Exists(ConfigurationFilePath))
            {
                try
                {
                    rawData = File.ReadAllText(ConfigurationFilePath);
                    return true;
                }
                catch (Exception)
                {
                }
            }
            rawData = null;
            return false;
        }

        public bool TrySaveRaw(string rawData)
        {
            try
            {
                File.WriteAllText(ConfigurationFilePath, rawData);
                return true;
            }
            catch (Exception)
            {}

            return false;
        }
    }
}
