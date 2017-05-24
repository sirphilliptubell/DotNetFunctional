namespace System
{
    /// <summary>
    /// Represents an object which contains an error message.
    /// </summary>
    public interface IErrorContainer
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        string Error { get; }
    }
}
