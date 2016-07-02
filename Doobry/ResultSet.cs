using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Doobry
{
    public class ResultSet
    {
        public ResultSet(IEnumerable<Result> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));

            Results = new ReadOnlyCollectionBuilder<Result>(results).ToReadOnlyCollection();
        }

        public ResultSet(string error)
        {
            Error = error;
        }

        public string Error { get; }


        public IReadOnlyCollection<Result> Results { get; }
    }
}