using System.Diagnostics;

namespace System
{
    /// <summary>
    /// The shared logic used in both Result and Result&lt;T&gt;.
    /// </summary>
    internal sealed class ResultCommonLogic
    {
        /// <summary>
        /// Gets a value indicating whether this instance represents a failure.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is failure; otherwise, <c>false</c>.
        /// </value>
        public bool IsFailure { get; }

        /// <summary>
        /// Gets a value indicating whether this instance represents a success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess
            => !IsFailure;

        /// <summary>
        /// The error if the instance is a failure.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _error;

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        /// <exception cref="InvalidOperationException">There is no error message for success.</exception>
        public string Error
        {
            [DebuggerStepThrough]
            get
                => IsSuccess
                ? throw new InvalidOperationException("There is no error message for success.")
                : _error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultCommonLogic"/> class.
        /// </summary>
        /// <param name="isFailure">if set to <c>true</c> the instance should represent a failure.</param>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">error - There must be error message for failure.</exception>
        /// <exception cref="ArgumentException">There should be no error message for success. - error</exception>
        [DebuggerStepThrough]
        public ResultCommonLogic(bool isFailure, string error)
        {
            if (isFailure)
            {
                if (string.IsNullOrEmpty(error))
                    throw new ArgumentNullException(nameof(error), "There must be error message for failure.");
            }
            else
            {
                if (error != null)
                    throw new ArgumentException("There should be no error message for success.", nameof(error));
            }

            IsFailure = isFailure;
            _error = error;
        }
    }
}
