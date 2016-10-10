using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Doobry
{
    // courtesy of https://github.com/LeeCampbell/RxCookbook/blob/master/Model/PropertyChange.md

    public static class PropertyExtensions
    {
        /// <summary>
        /// Gets property information for the specified <paramref name="property"/> expression.
        /// </summary>
        /// <typeparam name="TSource">Type of the parameter in the <paramref name="property"/> expression.</typeparam>
        /// <typeparam name="TValue">Type of the property's value.</typeparam>
        /// <param name="property">The expression from which to retrieve the property information.</param>
        /// <returns>Property information for the specified expression.</returns>
        /// <exception cref="ArgumentException">The expression is not understood.</exception>
        public static PropertyInfo GetPropertyInfo<TSource, TValue>(this Expression<Func<TSource, TValue>> property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var body = property.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Expression is not a property", nameof(property));
            }

            var propertyInfo = body.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Expression is not a property", nameof(property));
            }

            return propertyInfo;
        }
    }
}