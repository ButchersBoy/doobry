using System;

namespace Doobry.Settings
{
    public interface IQueryFileService
    {
        string GetFileName(Guid fileId);
    }
}