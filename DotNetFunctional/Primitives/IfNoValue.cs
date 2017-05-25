using System.Diagnostics.Contracts;

namespace System
{
    /// <summary>
    /// Hidden implementation for IIfNoValue.
    /// </summary>
    /// <seealso cref="System.IIfNoValue" />
    internal class IfNoValue
        : IIfNoValue
    {
        private readonly bool _ElseIsExecuted;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfNoValue"/> class.
        /// </summary>
        /// <param name="elseIsExecuted">if set to <c>true</c>, the Else() method will get executed.</param>
        internal IfNoValue(bool elseIsExecuted)
            => _ElseIsExecuted = elseIsExecuted;

        /// <summary>
        /// Performs an action when a Maybe object has no value.
        /// </summary>
        /// <param name="action">The action. Can't be null.</param>
        public void Else(Action action)
        {
            Contract.Requires<ArgumentNullException>(action != null, nameof(action));

            if (_ElseIsExecuted)
                action();
        }
    }
}