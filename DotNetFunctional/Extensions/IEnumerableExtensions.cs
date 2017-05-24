using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Gets only the errors of the specified Result objects.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<string> OnlyErrors(this IEnumerable<Result> results)
            => results
            .Where(x => x.IsFailure)
            .Select(x => x.Error);

        /// <summary>
        /// Gets only the errors of the specified Result objects.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<string> OnlyErrors<T>(this IEnumerable<Result<T>> results)
            => results
            .Where(x => x.IsFailure)
            .Select(x => x.Error);

        /// <summary>
        /// Returns the first item if there's only one item in the collection.
        /// Otherwise returns default(T).
        /// Unlike .Single() this will not throw an exception if there is more than one item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">Can't be null.</param>
        /// <returns></returns>
        public static Maybe<T> OnlyOneOrMaybe<T>(this IEnumerable<T> items)
            where T : class
        {
            Contract.Requires<ArgumentNullException>(items != null, nameof(items));

            var found = items.Take(2).ToList();

            return found.Count == 1
                ? found.First()
                : default(T);
        }

        /// <summary>
        /// Returns the first item if there's only one item in the collection.
        /// Otherwise returns default(T).
        /// Unlike .Single() this will not throw an exception if there is more than one item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">Can't be null.</param>
        /// <returns></returns>
        public static T? OnlyOneOrNullable<T>(this IEnumerable<T> items)
            where T : struct
        {
            Contract.Requires<ArgumentNullException>(items != null, nameof(items));

            var found = items.Take(2).ToList();

            return found.Count == 1
                ? found.First()
                : default(T);
        }

        /// <summary>
        /// Returns the first item if there's only one item in the collection.
        /// Otherwise returns default(T).
        /// Unlike .Single() this will not throw an exception if there is more than one item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">Can't be null.</param>
        /// <returns></returns>
        public static Result<T> OnlyOneOrResult<T>(this IEnumerable<T> items, string errorIfNotOne)
        {
            Contract.Requires<ArgumentNullException>(items != null, nameof(items));

            var found = items.Take(2).ToList();

            return found.Count == 1
                ? Result.Ok(found.First())
                : Result.Fail<T>(errorIfNotOne);
        }

        /// <summary>
        /// Gets only the success values of the specified Result objects.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> OnlyValues<T>(this IEnumerable<Result<T>> results)
            => results
            .Where(x => x.IsSuccess)
            .Select(x => x.Value);

        /// <summary>
        /// Filters the Maybe objects to only entries that have a value and returns those values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The maybes.</param>
        /// <returns></returns>
        public static IEnumerable<T> OnlyValues<T>(this IEnumerable<Maybe<T>> items)
            where T : class
            => items
            .Where(x => x.HasValue)
            .Select(x => x.Value);

        /// <summary>
        /// Returns either the only element in the sequence, or Maybe without a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static Maybe<T> SingleOrMaybe<T>(this IEnumerable<T> items)
            where T : class
            => items.SingleOrDefault();
    }
}