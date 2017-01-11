using System;
using System.IO;

namespace Doobry.Settings
{
    public class QueryFileService : IQueryFileService
    {
        public string GetFileName(Guid fileId)
        {
            return Path.Combine(Persistance.QueryFileFolder, Path.ChangeExtension(fileId.ToString(), "sql"));
        }
    }
}