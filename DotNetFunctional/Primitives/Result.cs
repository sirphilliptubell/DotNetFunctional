using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Represents a Success / Failure Result of some Action.
    /// </summary>
    public struct Result
    {
        /// <summary>
        /// The standard OK Result.
        /// </summary>
        private static readonly Result OkResult = new Result(false, null);

        /// <summary>
        /// The logic of Result.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ResultCommonLogic _logic;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> struct.
        /// </summary>
        /// <param name="isFailure">if set to <c>true</c> this instance represents a failure.</param>
        /// <param name="error">The error. Required (can't be null/empty) if a failure.</param>
        /// <exception cref="ArgumentNullException">error - There must be error message for failure.</exception>
        /// <exception cref="ArgumentException">There should be no error message for success. - error</exception>
        [DebuggerStepThrough]
        private Result(bool isFailure, string error)
            => _logic = new ResultCommonLogic(isFailure, error);

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        /// <exception cref="InvalidOperationException">If IsSuccess is true / IsFailure is false</exception>
        public string Error
            => _logic.Error;

        /// <summary>
        /// Gets a value indicating whether an action failed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is failure; otherwise, <c>false</c>.
        /// </value>
        public bool IsFailure
            => _logic.IsFailure;

        /// <summary>
        /// Gets a value indicating whether an action succeeded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess
            => _logic.IsSuccess;

        /// <summary>
        /// Returns a new fail result.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">error - There must be error for a failure.</exception>
        [DebuggerStepThrough]
        public static Result Fail(string error)
            => new Result(true, error);

        /// <summary>
        /// Returns a new fail result.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">There must be an exception for a failure.</exception>
        [DebuggerStepThrough]
        public static Result Fail(Exception ex)
            => new Result(true, ex?.Message);

        /// <summary>
        /// Creates a Fail Result using the specified type and error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="error">The error.</param>
        /// <exception cref="ArgumentNullException">error - There must be error for a failure.</exception>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Result<T> Fail<T>(string error)
            => new Result<T>(true, default(T), error);

        /// <summary>
        /// Creates a Fail Result using the specified type and error message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex">The Exception.</param>
        /// <exception cref="ArgumentNullException">There must be an exception for a failure.</exception>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Result<T> Fail<T>(Exception ex)
            => new Result<T>(true, default(T), ex?.Message);

        /// <summary>
        /// Creates a Fail result for the specified type and error.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Result<T> Fail<T>(IErrorContainer error)
            => new Result<T>(true, default(T), error.Error);

        /// <summary>
        /// Returns a Success Result.
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Result Ok()
            => OkResult;

        /// <summary>
        /// Creates a Success Result using the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static Result<T> Ok<T>(T value)
            => new Result<T>(false, value, null);

        /// <summary>
        /// If this Result is a Failure, the same Result is returned.
        /// If this Result is a Success, returns a new Failure Result if the condition is
        /// false, otherwise returns a Success Result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="condition">The condition to check if this instance is a Success.</param>
        /// <param name="error">The error message to use if the condition is false.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result Ensure(bool condition, string error)
        {
            if (IsFailure)
                return this;
            else //success
            {
                return
                    condition
                    ? Result.Ok()
                    : Result.Fail(error);
            }
        }

        /// <summary>
        /// If this Result is a Failure, the same Result is returned.
        /// If this Result is a Success, returns a new Failure Result if the predicate is
        /// false, otherwise returns a Success Result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="predicate">The predicate to check if this instance is a Success.</param>
        /// <param name="error">The error message to use if the predicate is false.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result Ensure(Func<bool> predicate, string error)
        {
            if (IsFailure)
                return this;
            else //success
            {
                return
                    predicate()
                    ? Result.Ok()
                    : Result.Fail(error);
            }
        }

        /// <summary>
        /// Returns a new Result regardless of whether this Result is a Success or Failure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <param name="onBoth">The function whose result to return.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public T OnBoth<T>(Func<Result, T> onBoth)
            => onBoth(this);

        /// <summary>
        /// Performs the specified Action if the Result is a Failure.
        /// Returns the same Result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <param name="whenFailure">Called when the Result is a Failure.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result OnFailure(Action<string> whenFailure)
        {
            if (IsFailure)
                whenFailure(Error);

            return this;
        }

        /// <summary>
        /// Performs the specified Action if the Result is a Failure.
        /// Returns the same Result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="whenFailure">Called if the Result is a Failure.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result OnFailure(Action whenFailure)
        {
            if (IsFailure)
                whenFailure();

            return this;
        }

        /// <summary>
        /// If this Result is a Success, returns a new Success Result with the specified Value.
        /// If this Result is a Failure, returns the fail result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <param name="newSuccessValue">The new Success value to return.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> OnSuccess<T>(T newSuccessValue)
            => IsSuccess
            ? Result.Ok(newSuccessValue)
            : Result.Fail<T>(Error);

        /// <summary>
        /// If this Result is a Success, Returns a Result using the return
        /// value of the specified function.
        /// If this Result is a failure, returns the same Fail Result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <param name="getResult">Gets a new Result when the current Result is a Success.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> OnSuccess<T>(Func<Result<T>> getResult)
            => IsSuccess
            ? getResult()
            : Result.Fail<T>(Error);

        /// <summary>
        /// If this Result is a Success, Returns a Success Result using the return
        /// value of the specified function.
        /// If this Result is a failure, returns the same Fail Result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <param name="getSuccessValue">Gets a new Success value when the current Result is a Success.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> OnSuccess<T>(Func<T> getSuccessValue)
            => IsSuccess
            ? Result.Ok(getSuccessValue())
            : Result.Fail<T>(Error);

        /// <summary>
        /// If this Result is a Failure, the same result is returned.
        /// If this Result is a Success, the Result from the specified function is returned.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="getResult">Gets the Result to return if the current Result is a Success.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result OnSuccess(Func<Result> getResult)
            => IsSuccess
            ? getResult()
            : this;

        /// <summary>
        /// If this Result is a Success, performs the specified action.
        /// Returns this instance.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result OnSuccessTee(Action action)
        {
            if (IsSuccess)
                action();

            return this;
        }

        /// <summary>
        /// Converts the Result into a Result&lt;T&gt; of the specified Type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> ToTypedResult<T>(T item = default(T))
            => this.IsFailure
            ? Result.Fail<T>(this.Error)
            : Result.Ok(item);
    }
}