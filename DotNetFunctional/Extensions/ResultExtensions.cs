using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// Extension methods for Result.
    /// </summary>
    public static class ResultExtensions
    {
        internal const string DEFAULT_SEPARATOR = ", ";

        /// <summary>
        /// Combines the string into a single string using the specified separator.
        /// </summary>
        /// <param name="strings">The errors.</param>
        /// <param name="separator">The error message separator.</param>
        /// <returns></returns>
        private static string CombineErrors(IEnumerable<string> strings, string separator)
            => strings
            .Aggregate(
                seed: new StringBuilder(),
                func: (sb, s) => sb.Append(s, separator),
                resultSelector: sb => sb.ToString());

        /// <summary>
        /// Segregates the items into Errors and Values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        private static (IEnumerable<string> errors, IEnumerable<T> successItems) Segregate<T>(this IEnumerable<Result<T>> items)
        {
            var errors = new List<string>();
            var successItems = new List<T>();

            foreach (var item in items)
            {
                if (item.IsFailure)
                    errors.Add(item.Error);
                else
                    successItems.Add(item.Value);
            }

            return (errors, successItems);
        }

        #region CombineAll()

        /// <summary>
        /// Combines all the Results in the <paramref name="results" /> list.
        /// If there are no failure results, a Success Result is returned.
        /// If there are any failure results, a Failure Result is returned that contains all the
        /// error messages (separated separated by <paramref name="errorMessagesSeparator" />.)
        /// </summary>
        /// <param name="errorMessagesSeparator">Separator for error messages. Defaults to a comma.</param>
        /// <param name="results">The results to combine.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Result CombineAll(this IEnumerable<Result> results, string errorMessagesSeparator = DEFAULT_SEPARATOR)
        {
            Contract.Requires<ArgumentNullException>(results != null, nameof(results));
            Contract.Requires<ArgumentNullException>(errorMessagesSeparator != null, nameof(errorMessagesSeparator));

            var errorSummary = CombineErrors(results.OnlyErrors(), errorMessagesSeparator);

            return
                errorSummary == string.Empty
                ? Result.Ok()
                : Result.Fail(errorSummary);
        }

        /// <summary>
        /// Combines the results into a single result.
        /// Returns all the errors in a single result if there are any.
        /// Returns all the items as a success if there aren't any errors.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="errorMessagesSeparator">The error messages separator.</param>
        /// <param name="results">The results to combine.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Result<IEnumerable<T>> CombineAll<T>(this IEnumerable<Result<T>> results, string errorMessagesSeparator = DEFAULT_SEPARATOR)
        {
            Contract.Requires<ArgumentNullException>(results != null, nameof(results));
            Contract.Requires<ArgumentNullException>(errorMessagesSeparator != null, nameof(errorMessagesSeparator));

            //separate everything
            var segregated = Segregate(results);

            return segregated.errors.Any()
                ? Result.Fail<IEnumerable<T>>(
                    CombineErrors(segregated.errors, errorMessagesSeparator))
                : Result.Ok(segregated.successItems);
        }

        #endregion CombineAll()

        #region CombineSequential()

        /// <summary>
        /// Combines the Results, one at a time.
        /// Returns the first failure encountered, or a Success Result if there are no failures.
        /// </summary>
        /// <param name="results">The results to combine.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Result CombineSequential(this IEnumerable<Result> results)
        {
            Contract.Requires<ArgumentNullException>(results != null, nameof(results));

            foreach (var result in results)
            {
                if (result.IsFailure)
                    return result;
            }

            return Result.Ok();
        }

        /// <summary>
        /// Combines the Results, one at a time.
        /// Returns the first failure encountered, or a Success Result if there are no failures.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="results">The results to combine.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Result CombineSequential<T>(this IEnumerable<Result<T>> results)
        {
            Contract.Requires<ArgumentNullException>(results != null, nameof(results));

            foreach (var result in results)
            {
                if (result.IsFailure)
                    return result;
            }

            return Result.Ok();
        }

        /// <summary>
        /// Combines the Results, one at a time.
        /// Returns the first failure encountered, or a Success Result if there are no failures.
        /// </summary>
        /// <param name="results">The results to combine.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Result CombineSequential(this IEnumerable<Func<Result>> functions)
        {
            Contract.Requires<ArgumentNullException>(functions != null, nameof(functions));
            Contract.Requires<ArgumentException>(!functions.Any(x => x == null), "one of the functions is null");

            foreach (var function in functions)
            {
                var result = function();
                if (result.IsFailure)
                    return result;
            }

            return Result.Ok();
        }

        /// <summary>
        /// Combines the Results of each function, one at a time.
        /// Returns the first failure encountered, or all the results if there are no failures.
        /// </summary>
        /// <param name="results">The results to combine.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<Result<T>> CombineSequential<T>(this IEnumerable<Func<Result<T>>> functions)
        {
            Contract.Requires<ArgumentNullException>(functions != null, nameof(functions));
            Contract.Requires<ArgumentException>(!functions.Any(x => x == null), "one of the functions is null");

            var results = new List<Result<T>>();

            foreach (var function in functions)
            {
                var r = function();
                if (r.IsFailure)
                {
                    yield return r;
                    yield break;
                }
            }

            foreach (var r in results)
                yield return r;
        }

        #endregion CombineSequential()

        /// <summary>
        /// Alters all the items in the list with the specified function.
        /// Returns the same (and altered) list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="alterEntry"></param>
        /// <returns></returns>
        public static List<T> AlterInPlace<T>(this List<T> list, Func<T, T> alterEntry)
        {
            Contract.Requires<ArgumentNullException>(list != null, nameof(list));
            Contract.Requires<ArgumentNullException>(alterEntry != null, nameof(alterEntry));

            for (int i = 0; i < list.Count; i++)
                list[i] = alterEntry(list[i]);

            return list;
        }

        /// <summary>
        /// Alters all the items in the list with the specified function.
        /// Returns the same (and altered) list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="alterEntry"></param>
        /// <returns></returns>
        public static T[] AlterInPlace<T>(this T[] list, Func<T, T> alterEntry)
        {
            Contract.Requires<ArgumentNullException>(list != null, nameof(list));
            Contract.Requires<ArgumentNullException>(alterEntry != null, nameof(alterEntry));

            for (int i = 0; i < list.Length; i++)
                list[i] = alterEntry(list[i]);

            return list;
        }

        /// <summary>
        /// Performs the specified action with each item in the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="action">The action to perform on each item.</param>
        /// <returns></returns>
        public static IEnumerable<T> Tee<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        /// Performs the specified action with each item in the collection if the condition is true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="condition">The condition to be true for the action to be performed.</param>
        /// <param name="action">The action to perform if the condition is true.</param>
        /// <returns></returns>
        public static IEnumerable<T> Tee<T>(this IEnumerable<T> items, bool condition, Action<T> action)
        {
            if (condition)
            {
                foreach (var item in items)
                {
                    action(item);
                    yield return item;
                }
            }
            else
            {
                foreach (var item in items)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Performs the specified action with each item in the collection if the condition is true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="predicate">The predicate that must be true for the action to be performed.</param>
        /// <param name="action">The action to perform if the predicate is true.</param>
        /// <returns></returns>
        public static IEnumerable<T> Tee<T>(this IEnumerable<T> items, Func<T, bool> predicate, Action<T> action)
        {
            foreach (var item in items)
            {
                if (predicate(item))
                    action(item);
                yield return item;
            }
        }
    }
}