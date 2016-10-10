using System;

namespace Doobry.Infrastructure
{
    public class SortExpression<T>
    {
        public SortExpression(Func<T, IComparable> expression, SortDirection direction = SortDirection.Ascending)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            Expression = expression;
            Direction = direction;
        }

        public SortDirection Direction { get; }

        public Func<T, IComparable> Expression { get; }
    }
}