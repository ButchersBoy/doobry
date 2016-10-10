using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

//portions of this class taken from https://github.com/LeeCampbell/RxCookbook/blob/master/Model/PropertyChange.md

namespace Doobry
{
    public static class NotifyPropertyChangedExtension
    {
        public static void MutateVerbose<TField>(this INotifyPropertyChanged instance, ref TField field, TField newValue, Action<PropertyChangedEventArgs> raise, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TField>.Default.Equals(field, newValue)) return;
            field = newValue;
            raise?.Invoke(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Returns an observable sequence of the source any time the <c>PropertyChanged</c> event is raised.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <returns>Returns an observable sequence of the value of the source when ever the <c>PropertyChanged</c> event is raised.</returns>
        public static IObservable<T> OnAnyPropertyChanged<T>(this T source)
            where T : INotifyPropertyChanged
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => handler.Invoke,
                    h => source.PropertyChanged += h,
                    h => source.PropertyChanged -= h)
                .Select(_ => source);
        }

        /// <summary>
        /// Returns an observable sequence of the value of a property when <paramref name="source"/> raises <seealso cref="INotifyPropertyChanged.PropertyChanged"/> for the given property.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property that is being observed.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <param name="property">An expression that describes which property to observe.</param>
        /// <returns>Returns an observable sequence of the property values as they change.</returns>
        public static IObservable<TProperty> OnPropertyChanged<T, TProperty>(this T source, Expression<Func<T, TProperty>> property)
            where T : INotifyPropertyChanged
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            return Observable.Create<TProperty>(o =>
            {
                var propertyName = property.GetPropertyInfo().Name;
                var propertySelector = property.Compile();

                return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                            .Where(e => e.EventArgs.PropertyName == propertyName)
                            .Select(e => propertySelector(source))
                            .Subscribe(o);
            });
        }
    }
}