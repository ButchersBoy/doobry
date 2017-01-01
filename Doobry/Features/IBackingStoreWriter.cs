using Newtonsoft.Json.Linq;

namespace Doobry.Features
{
    public interface IBackingStoreWriter
    {
        void WriteToBackingStore(object tabContentViewModel, JToken into);
    }
}