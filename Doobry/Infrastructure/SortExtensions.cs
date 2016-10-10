using System;

namespace Doobry.Infrastructure
{
    public static class SortExtensions
    {
        public static SortExpression<TItem> Ascending<TItem>(this Func<TItem, IComparable> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            return new SortExpression<TItem>(expression);
        }

        public static SortExpression<TItem> Descending<TItem>(this Func<TItem, IComparable> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            return new SortExpression<TItem>(expression, SortDirection.Descending);
        }
    }
}