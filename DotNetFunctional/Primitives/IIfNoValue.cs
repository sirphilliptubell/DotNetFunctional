namespace System
{
    /// <summary>
    /// Allows taking an action when a Maybe object doesn't have a value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IIfNoValue
    {
        /// <summary>
        /// Performs an action when a Maybe object has no value.
        /// </summary>
        /// <param name="action">The action. Can't be null.</param>
        void Else(Action action);
    }
}
