using System.Diagnostics;

namespace System
{
    /// <summary>
    /// Represents a Success / Failure Result of some Action that may contain a value.
    /// </summary>
    public struct Result<T>
    {
        /// <summary>
        /// The logic of Result.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ResultCommonLogic _logic;

        /// <summary>
        /// The value if this instance is a success.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly T _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Result" /> struct.
        /// </summary>
        /// <param name="isFailure">if set to <c>true</c> this instance represents a failure.</param>
        /// <param name="value">The value. Can't be null if a success.</param>
        /// <param name="error">The error. Required (can't be null/empty) if a failure.</param>
        /// <exception cref="ArgumentNullException">error - There must be error message for failure.</exception>
        /// <exception cref="ArgumentException">There should be no error message for success. - error</exception>
        [DebuggerStepThrough]
        internal Result(bool isFailure, T value, string error)
        {
            if (!isFailure && value == null)
                throw new ArgumentNullException(nameof(value));

            _logic = new ResultCommonLogic(isFailure, error);
            _value = value;
        }

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
        /// Gets the value associated witht the result.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">There is no value for failure.</exception>
        public T Value
        {
            [DebuggerStepThrough]
            get
                => IsSuccess
                ? _value
                : throw new InvalidOperationException("There is no value for failure.");
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
            => IsSuccess
            ? "Ok: " + Value.ToString()
            : "Failure: " + Error;

        /// <summary>
        /// Converts this generic typed Result into a standard Result.
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result ToResult()
            => IsFailure
            ? Result.Fail(Error)
            : Result.Ok();

        /// <summary>
        /// Converts the Result of this type into a Result of another type.
        /// If this instance is a failure, a new result of the new type is returned with the same error.
        /// If this instance is a success, a new result with the mapped type is returned as a success.
        /// </summary>
        /// <typeparam name="U">The type to map to</typeparam>
        /// <param name="mapSuccessValue">A function that converts the success value.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<U> ToTypedResult<U>(Func<T, U> mapSuccessValue)
            => this.IsFailure
            ? Result.Fail<U>(this.Error)
            : Result.Ok(mapSuccessValue(this.Value));

        /// <summary>
        /// Performs an implicit conversion from <see cref="Result{T}"/> to <see cref="Result"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [DebuggerStepThrough]
        public static implicit operator Result(Result<T> result)
            => result.IsSuccess
            ? Result.Ok()
            : Result.Fail(result.Error);

        /// <summary>
        /// If this Result is a Failure, the same Result is returned.
        /// If this Result is a Success, returns a new Failure Result if the predicate is
        /// false, otherwise returns the same Success Result.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="error">The error message.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> Ensure(Func<T, bool> predicate, string error)
        {
            if (IsFailure)
                return Result.Fail<T>(Error);
            else //success
            {
                return
                    predicate(Value)
                    ? this
                    : Result.Fail<T>(error);
            }
        }

        /// <summary>
        /// If this Result is a Failure, the same Result is returned.
        /// If this Result is a Success, returns a new Result if the condition is
        /// false, otherwise returns the original Success Result.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="whenFalse">The when false.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> Ensure(bool condition, Func<T, Result<T>> whenFalse)
        {
            if (IsFailure)
                return Result.Fail<T>(Error);
            else //success
            {
                return
                    condition
                    ? this
                    : whenFalse(Value);
            }
        }

        /// <summary>
        /// Returns a new Result regardless of whether this Result is a Success or Failure.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="onBoth">The function whose result to return.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public U OnBoth<U>(Func<Result<T>, U> onBoth)
            => onBoth(this);

        /// <summary>
        /// Performs the specified Action if the Result is a Failure.
        /// Returns the same Result.
        /// </summary>
        /// <param name="whenFailure">Called when the Result is a Failure.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> OnFailure(Action<string> whenFailure)
        {
            if (IsFailure)
                whenFailure(Error);

            return this;
        }

        /// <summary>
        /// If this Result is a Success, returns the same Result.
        /// If this Result is a Failure, returns a new Success Result with the specified value.
        /// </summary>
        /// <param name="whenFailure">The Success value to use when failure.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> OnFailure(T whenFailure)
            => IsSuccess
            ? this
            : Result.Ok(whenFailure);

        /// <summary>
        /// Performs the specified Action if the Result is a Failure.
        /// Returns the same Result.
        /// </summary>
        /// <param name="whenFailure">Called if the Result is a Failure.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> OnFailure(Action whenFailure)
        {
            if (IsFailure)
                whenFailure();

            return this;
        }

        /// <summary>
        /// If this Result is a Failure, returns the same Result.
        /// If this Result is a Success, maps the Success value to a new Success value.
        /// </summary>
        /// <param name="map">A function that returns a new result given the current instance's success value.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result OnSuccess(Func<T, Result> map)
        {
            if (IsFailure)
                return Result.Fail(Error);

            return map(Value);
        }

        /// <summary>
        /// If this Result is a Failure, returns the same result.
        /// If this Result is a Success, returns the new Result from the specified function.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="nextResult">Gets the Result to return when the current Result is a Success.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<U> OnSuccess<U>(Func<Result<U>> nextResult)
        {
            if (IsFailure)
                return Result.Fail<U>(Error);

            return nextResult();
        }

        /// <summary>
        /// If this Result is a Success, returns a new Result with the value
        /// provided by the specified function.
        /// If this Result is a Failure, returns the same result.
        /// </summary>
        /// <param name="getResult">Gets the new Result when the current Result is a Success.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result OnSuccess(Func<Result> getResult)
            => IsSuccess
            ? getResult()
            : Result.Fail(Error);

        /// <summary>
        /// If this Result is a Success, returns the result of the
        /// specified function when passed the success value.
        /// If this Result is a Failure, returns the same result.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="map">Gets the result to return, argument is the current success value.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<U> OnSuccess<U>(Func<T, Result<U>> map)
            => IsSuccess
            ? map(Value)
            : Result.Fail<U>(Error);

        /// <summary>
        /// If this Result is a Failure, returns the same result.
        /// If this Result is a Success, returns a Success Result using the value from
        /// the specified function.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="mapSuccessValue">Gets the new Success value to use when the current Result is a Success.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<U> OnSuccess<U>(Func<T, U> mapSuccessValue)
            => IsSuccess
            ? Result.Ok(mapSuccessValue(Value))
            : Result.Fail<U>(Error);

        /// <summary>
        /// If this Result is a Failure, the same Result is returned.
        /// If this Result is a Success, returns a new Result if the condition is
        /// true, otherwise returns the same Success Result.
        /// </summary>
        /// <param name="condition">the condition to check.</param>
        /// <param name="whenTrue">The function whose value to return if the condition is true.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> OnSuccess(
            bool condition,
            Func<T, Result<T>> whenTrue)
        {
            if (IsFailure)
                return Result.Fail<T>(Error);
            else //success
            {
                if (condition)
                    return whenTrue(Value);

                return this;
            }
        }

        /// <summary>
        /// If this Result is a Success, performs the specified action.
        /// Returns this instance.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> OnSuccessTee(Action action)
        {
            if (IsSuccess)
                action();

            return this;
        }

        /// <summary>
        /// If this Result is a Success, performs the specified action using the success Value.
        /// Returns the same result.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Result<T> OnSuccessTee(Action<T> action)
        {
            if (IsSuccess)
                action(Value);

            return this;
        }
    }
}