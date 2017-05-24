using System.Text;

namespace System
{
    /// <summary>
    /// Extension methods for StringBuilder.
    /// </summary>
    internal static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends the specified text.
        /// However this will first append the specified separator if the StringBuilder already has anything.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="text">The text.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        internal static StringBuilder Append(this StringBuilder sb, string text, string separator)
            => sb.Length == 0
            ? sb.Append(text)
            : sb.Append(separator).Append(text);
    }
}
