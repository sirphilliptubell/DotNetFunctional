using System.Diagnostics.Contracts;

namespace System
{
    /// <summary>
    /// Helper class for Maybe{T}.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Performs a manual conversion from <see cref="T"/> to <see cref="Maybe{T}"/>.
        /// Useful for cases where Visual Studio doesn't allow implicit conversion
        /// due to using interfaces.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static Maybe<T> From<T>(T obj)
            where T : class
            => obj;
    }
}