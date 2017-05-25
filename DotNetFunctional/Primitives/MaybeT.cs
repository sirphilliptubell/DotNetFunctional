using System.Diagnostics.Contracts;

namespace System
{
    /// <summary>
    /// Wrapper for a reference type which may or may not be null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.IEquatable{System.Maybe{T}}" />
    public struct Maybe<T> :
        IEquatable<Maybe<T>>
        where T : class
    {
        /// <summary>
        /// The string returned when ToString() is called when there is no value.
        /// </summary>
        public const string NO_VALUE_STRING = "[No Value]";

        /// <summary>
        /// The wrapped value.
        /// </summary>
        private readonly T _value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <exception cref="InvalidOperationException">This object has no value.</exception>
        public T Value
            => HasNoValue
            ? throw new InvalidOperationException("This object has no value.")
            : _value;

        /// <summary>
        /// Gets a value indicating whether this instance has a value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has a value; otherwise, <c>false</c>.
        /// </value>
        public bool HasValue =>
            _value != null;

        /// <summary>
        /// Gets a value indicating whether this instance has no value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has no value; otherwise, <c>false</c>.
        /// </value>
        public bool HasNoValue
            => !HasValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Maybe{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        private Maybe(T value)
            => _value = value;

        /// <summary>
        /// Performs an implicit conversion from T to <see cref="Maybe{T}" />.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Maybe<T>(T value)
            => new Maybe<T>(value);

        /// <summary>
        /// Gets the value or returns the specified default value.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public T GetValueOrDefault(T defaultValue)
            => this.HasValue
            ? this.Value
            : defaultValue;

        /// <summary>
        /// Performs an action if there is a value.
        /// </summary>
        /// <param name="action">The action to perform with the value. Can't be null.</param>
        /// <returns></returns>
        public IIfNoValue IfValue(Action<T> action)
        {
            Contract.Requires<ArgumentNullException>(action != null, nameof(action));

            if (this.HasValue)
                action(this.Value);

            return new IfNoValue(elseIsExecuted: this.HasNoValue);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="maybe">The maybe object.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Maybe<T> maybe, T value)
            => maybe.HasValue
            ? maybe.Value == value
            : false;

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="maybe">The maybe object.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Maybe<T> maybe, T value)
            => !(maybe == value);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="maybe">The maybe object.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(T value, Maybe<T> maybe)
            => maybe.HasValue
            ? value == maybe.Value
            : false;

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="maybe">The maybe object.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(T value, Maybe<T> maybe)
            => !(value == maybe);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="first">The first maybe object.</param>
        /// <param name="second">The second maybe object.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Maybe<T> first, Maybe<T> second)
            => first.Value == second.Value;

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="first">The first maybe object.</param>
        /// <param name="second">The second maybe object.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Maybe<T> first, Maybe<T> second)
            => !(first == second);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (this.HasNoValue)
                return false;
            else if (obj is Maybe<T> asMaybe)
                return this.Equals(asMaybe);
            else
                return _value.Equals(obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Maybe<T> other)
            => HasNoValue || other.HasNoValue
            ? false
            : Value.Equals(other.Value);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
            => HasNoValue
            ? 0
            : Value.GetHashCode();

        /// <summary>
        /// Returns NO_VALUE_STRING if this instance has no value, otherwise returns the string that represents the Value {T}.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
            => HasValue
            ? Value.ToString()
            : NO_VALUE_STRING;

        /// <summary>
        /// Returns the value if there is one.
        /// Returns the default value of {T} otherwise.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public T Unwrap(T defaultValue = default(T))
            => HasValue
            ? Value
            : defaultValue;

        /// <summary>
        /// If this instance has a Value, validates the specified predicate is true.
        /// Returns the same value if the predicate is true.
        /// Returns No Value if this instance has no value or the predicate is false.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public Maybe<T> Ensure(Func<T, bool> predicate)
            => HasValue && predicate(Value)
            ? Value
            : null;

        /// <summary>
        /// Returns the value provided by the mapping function if there is a value.
        /// Returns the default value for {U} otherwise.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="map">The mapping function.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public Maybe<U> Map<U>(Func<T, U> map, U defaultValue = default(U))
            where U : class
            => HasValue
            ? map(Value)
            : defaultValue;

        /// <summary>
        /// Returns the value provided by the mapping function if there is a value.
        /// Returns the default value for {U} otherwise.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="map">The mapping function.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public Maybe<U> Map<U>(Func<T, Maybe<U>> map, U defaultValue = default(U))
            where U : class
            => HasValue
            ? map(Value)
            : defaultValue;

        /// <summary>
        /// Converts the Maybe type to a Result type.
        /// If the Maybe has a Value, a Success Result is returned, otherwise a Failure Result is returned with the specified error.
        /// </summary>
        /// <param name="errorIfNoValue">The error message.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">error - There must be error for a failure.</exception>
        public Result<T> ToResult(string errorIfNoValue)
            => HasValue
            ? Result.Ok(Value)
            : Result.Fail<T>(errorIfNoValue);
    }
}