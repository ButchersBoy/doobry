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
        private const string FileName = "doobry-settings-auto.doobry";
        public static readonly string FilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FileName);

        public bool TryLoadRaw(out string rawData)
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    rawData = File.ReadAllText(FilePath);
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
                File.WriteAllText(FilePath, rawData);
                return true;
            }
            catch (Exception)
            {}

            return false;
        }
    }
}
