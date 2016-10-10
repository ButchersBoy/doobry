using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doobry.Infrastructure
{
    public class SortExpressionComparer<TItem> : IComparer<TItem>
    {
        private readonly IList<SortExpression<TItem>> _expressions;

        public SortExpressionComparer(params SortExpression<TItem>[] expressions)
        {
            _expressions = expressions.ToList();
        }

        public SortExpressionComparer(IEnumerable<SortExpression<TItem>> expressions)
        {
            if (expressions == null) throw new ArgumentNullException("expressions");

            _expressions = expressions.ToList();
        }

        public SortExpressionComparer(params Func<TItem, IComparable>[] expressions)
        {
            _expressions = expressions.Select(f => f.Ascending()).ToList();
        }

        public SortExpressionComparer(IEnumerable<Func<TItem, IComparable>> expressions)
        {
            if (expressions == null) throw new ArgumentNullException("expressions");

            _expressions = expressions.Select(f => f.Ascending()).ToList();
        }

        internal SortExpressionComparer(IList<SortExpression<TItem>> expressions)
        {
            _expressions = expressions;
        }

        public static SortExpressionComparer<TItem> Ascending(Func<TItem, IComparable> expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");

            return new SortExpressionComparer<TItem>(expression.Ascending());
        }

        public SortExpressionComparer<TItem> ThenAscending(Func<TItem, IComparable> expression)
        {
            return new SortExpressionComparer<TItem>(_expressions.Union(new[] { expression.Ascending() }));
        }

        public static SortExpressionComparer<TItem> Descending(Func<TItem, IComparable> expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");

            return new SortExpressionComparer<TItem>(expression.Descending());
        }

        public SortExpressionComparer<TItem> ThenDescending(Func<TItem, IComparable> expression)
        {
            return new SortExpressionComparer<TItem>(_expressions.Union(new[] { expression.Descending() }));
        }

        public int Compare(TItem x, TItem y)
        {
            return _expressions
                .ToArray()
                .Select(expr => Compare(x, y, expr))
                .FirstOrDefault(result => result != 0);
        }

        private static int Compare(TItem x, TItem y, SortExpression<TItem> sortExpression)
        {
            var xValue = sortExpression.Expression(x);
            var yValue = sortExpression.Expression(y);

            if (ReferenceEquals(null, xValue) && ReferenceEquals(null, yValue))
                return 0;

            if (ReferenceEquals(null, xValue))
                return sortExpression.Direction == SortDirection.Ascending ? -1 : 1;
            if (ReferenceEquals(null, yValue))
                return sortExpression.Direction == SortDirection.Ascending ? 1 : -1;
            
            if (ReferenceEquals(xValue, yValue)) return 0;

            var num = xValue.CompareTo(yValue);
            if (num == 0) return 0;

            return sortExpression.Direction == SortDirection.Ascending ? num : -num;
        }
    }
}
