using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doobry.DocumentDb;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Xunit;

namespace Doobry.Tests
{
    public class DatabaseQueryThings
    {
        [Fact]
        public async void QueryDatabases()
        {
            var databases = CreateDocumentClient().CreateDatabaseQuery().AsEnumerable().ToList();

            /*
            var database = new Database()
            {
                Id = "HelloWorld"
            };
            await CreateDocumentClient().CreateDatabaseAsync(database);
            */

            var database = CreateDocumentClient().CreateDatabaseQuery().AsEnumerable().ToList().First();

            var documentCollection = new DocumentCollection()
            {
                Id = "Mynewcollection"
            };
            var documentCollectionAsync = await CreateDocumentClient().CreateDocumentCollectionAsync(database.SelfLink, documentCollection);

//            CreateDocumentClient().CreateDocumentCollectionQuery()


        }

        private static DocumentClient CreateDocumentClient()
        {
            return new DocumentClient(new Uri(LocalEmulator.Host), LocalEmulator.AuthorisationKey);
        }
    }
}
