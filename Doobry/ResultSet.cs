using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Doobry
{
    public class ResultSet
    {        
        private readonly ObservableCollection<Result> _results;

        public ResultSet(IEnumerable<Result> results)
        {            
            if (results == null) throw new ArgumentNullException(nameof(results));

            _results = new ObservableCollection<Result>(results);
            Results = new ReadOnlyObservableCollection<Result>(_results);            
        }

        public ResultSet(string error)
        {
            Error = error;            
        }

        public ResultSet(DocumentClientException documentClientException)
        {
            if (documentClientException == null) throw new ArgumentNullException(nameof(documentClientException));

            Error =
                $"Error: {documentClientException.Error}{Environment.NewLine}Message: {documentClientException.Message}{Environment.NewLine}Status Code: {documentClientException.StatusCode}{Environment.NewLine}";
        }

        public string Error { get; }

        public ReadOnlyObservableCollection<Result> Results { get; }

        public void Append(IEnumerable<string> results)
        {            
            foreach (var result in results)
                _results.Add(new Result(Results.Count, result));                
        }
    }
}