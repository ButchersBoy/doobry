using System;
using System.IO;

namespace Doobry.Settings
{
    public class QueryFileService : IQueryFileService
    {
        public string GetFileName(Guid tabId)
        {
            return Path.Combine(Persistance.QueryFileFolder, Path.ChangeExtension(tabId.ToString(), "sql"));
        }
    }
}