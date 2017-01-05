using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Kernel;

namespace Doobry.DocumentDb
{
    public static class LocalEmulator
    {
        //TODO allow port to be overriden
        public static string Host = "https://localhost:8081";

        public static string AuthorisationKey =
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    }
    
}
