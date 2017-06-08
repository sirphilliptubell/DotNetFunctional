namespace System
{
    /// <summary>
    /// Represents one of two different types of values.
    /// Which value is the proper value is indicated by the IsLeft and IsRight properties.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left.</typeparam>
    /// <typeparam name="TRight">The type of the right.</typeparam>
    public struct Either<TLeft, TRight>
        : IEquatable<Either<TLeft, TRight>>
    {
        private const byte LEFT_VALUE = 0b0000_0010;
        private const byte RGHT_VALUE = 0b0000_0001;

        /// <summary>
        /// Indicates whether this instance is in the Left or Right state.
        /// 1 = Right state
        /// 2 = Left state
        /// 0 if this struct was not initialized.
        /// </summary>
        private readonly byte _which;

        /// <summary>
        /// Initializes a new instance of the <see cref="Either{TLeft, TRight}"/> struct.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="isRight">if set to <c>true</c> then the instance will be in the Right state, otherwise the Left state.</param>
        /// <exception cref="System.ArgumentException">isLeft</exception>
        private Either(TLeft left, TRight right, bool isRight)
        {
            this.PeekLeft = left;
            this.PeekRight = right;

            if (isRight)
                _which = RGHT_VALUE;
            else
                _which = LEFT_VALUE;
        }

        /// <summary>
        /// Gets a value indicating whether this instance was not initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault
            => _which == 0;

        /// <summary>
        /// Gets a value indicating whether this instance is in the Left state.
        /// Throws an exception if this instance was never initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is left; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Either was never initialized</exception>
        public bool IsLeft
            => IsDefault
            ? throw new InvalidOperationException("Either was never initialized")
            : _which == LEFT_VALUE;

        /// <summary>
        /// Gets a value indicating whether this instance is in the Right state.
        /// Throws an exception if this instance was never initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is right; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Either was never initialized</exception>
        public bool IsRight
            => IsDefault
            ? throw new InvalidOperationException("Either was never initialized")
            : _which == RGHT_VALUE;

        /// <summary>
        /// Gets the Left value.
        /// Throws an exception if not in the Left state.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Cannot access the Left value when not in the Left state.</exception>
        public TLeft Left
            => IsLeft
            ? PeekLeft
            : throw new InvalidOperationException("Cannot access the Left value when not in the Left state.");

        /// <summary>
        /// Gets the Left value.
        /// Does not check to ensure this instance is in the Left state.
        /// </summary>
        public TLeft PeekLeft { get; }

        /// <summary>
        /// Gets the Right value.
        /// Does not check to ensure this instance is in the Right state.
        /// </summary>
        public TRight PeekRight { get; }

        /// <summary>
        /// Gets the Right.
        /// Throws an exception if not in the Right state.
        /// </summary>
        /// <value>
        /// The right value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Cannot access the Right value when not in the Right state.</exception>
        public TRight Right
            => IsRight
            ? PeekRight
            : throw new InvalidOperationException("Cannot access the Right value when not in the Right state.");

        /// <summary>
        /// Performs an implicit conversion from <see cref="TLeft"/> to <see cref="Either{TLeft, TRight}"/>.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Either<TLeft, TRight>(TLeft left)
            => new Either<TLeft, TRight>(left, default(TRight), isRight: false);

        /// <summary>
        /// Performs an implicit conversion from <see cref="TRight"/> to <see cref="Either{TLeft, TRight}"/>.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Either<TLeft, TRight>(TRight right)
            => new Either<TLeft, TRight>(default(TLeft), right, isRight: true);

        /// <summary>
        /// Returns a value depending on which state this instance is in.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="ifLeft">The method to perform if this instance is in the left state.</param>
        /// <param name="ifRight">The method to perform if this instance is in the right state.</param>
        /// <returns></returns>
        public TResult Return<TResult>(Func<TLeft, TResult> ifLeft, Func<TRight, TResult> ifRight)
        {
            if (ifLeft == null) throw new ArgumentNullException(nameof(ifLeft));
            if (ifRight == null) throw new ArgumentNullException(nameof(ifRight));
            if (IsDefault) throw new InvalidOperationException("Either was never initialized");

            return IsLeft
                ? ifLeft(this.PeekLeft)
                : ifRight(this.PeekRight);
        }

        /// <summary>
        /// Performs an action depending on which state this instance is in.
        /// </summary>
        /// <param name="ifLeft">The action to perform if this instance is in the left state.</param>
        /// <param name="ifRight">The action to perform if this instance is in the right state.</param>
        /// <exception cref="ArgumentNullException">
        /// ifLeft
        /// or
        /// ifRight
        /// </exception>
        public void Switch(Action<TLeft> ifLeft, Action<TRight> ifRight)
        {
            if (ifLeft == null) throw new ArgumentNullException(nameof(ifLeft));
            if (ifRight == null) throw new ArgumentNullException(nameof(ifRight));

            if (IsLeft)
                ifLeft(this.PeekLeft);
            else
                ifRight(this.PeekRight);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Either<TLeft, TRight> other)
            => _which == other._which
            && PeekLeft != null && PeekLeft.Equals(other.PeekLeft)
            && PeekRight != null && PeekRight.Equals(other.PeekRight);
    }
}