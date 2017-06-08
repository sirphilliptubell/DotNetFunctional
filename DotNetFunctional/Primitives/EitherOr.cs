namespace System
{
    /// <summary>
    /// Represents two different types of values.
    /// Any combination of which may be a be an acceptable value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left.</typeparam>
    /// <typeparam name="TRight">The type of the right.</typeparam>
    public struct EitherOr<TLeft, TRight>
    {
        private const byte BOTH_VALUE = 0b0000_0011;
        private const byte LEFT_VALUE = 0b0000_0010;
        private const byte RGHT_VALUE = 0b0000_0001;

        /// <summary>
        /// Indicates whether this instance is in the Left or Right state.
        /// 1 = Right state
        /// 2 = Left state
        /// 3 = Both state
        /// 0 = Neither state / not initialized.
        /// </summary>
        private readonly byte _which;

        /// <summary>
        /// Initializes a new instance of the <see cref="Either{TLeft, TRight}" /> struct.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="isLeft">if set to <c>true</c> the Left value is acceptable.</param>
        /// <param name="isRight">if set to <c>true</c> the Right value is acceptable.</param>
        /// <exception cref="System.ArgumentException">isLeft</exception>
        private EitherOr(TLeft left, TRight right, bool isLeft, bool isRight)
        {
            this.PeekLeft = left;
            this.PeekRight = right;

            _which = 0;
            if (isLeft)
                _which |= LEFT_VALUE;
            if (isRight)
                _which |= RGHT_VALUE;
        }

        /// <summary>
        /// Gets a value indicating whether both the Left and Right values are acceptable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is both; otherwise, <c>false</c>.
        /// </value>
        public bool IsBoth
            => _which == BOTH_VALUE;

        /// <summary>
        /// Gets a value indicating whether this instance is in the Left state.
        /// Throws an exception if this instance was never initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is left; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Either was never initialized</exception>
        public bool IsLeft
            => (_which & LEFT_VALUE) == LEFT_VALUE;

        /// <summary>
        /// Gets a value indicating whether this instance was not initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsNeither
            => _which == 0;

        /// <summary>
        /// Gets a value indicating whether this instance is in the Right state.
        /// Throws an exception if this instance was never initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is right; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Either was never initialized</exception>
        public bool IsRight
            => (_which & RGHT_VALUE) == RGHT_VALUE;

        /// <summary>
        /// Gets the Left value.
        /// Throws an exception if the Left value is not acceptable.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Cannot access the Left value when the Left value is not acceptable.</exception>
        public TLeft Left
            => IsLeft
            ? PeekLeft
            : throw new InvalidOperationException("Cannot access the Left value when not in the Left state.");

        /// <summary>
        /// Gets the Left value.
        /// Does not check to ensure the Right value is acceptable.
        /// </summary>
        public TLeft PeekLeft { get; }

        /// <summary>
        /// Gets the Right value.
        /// Does not check to ensure the Left value is acceptable.
        /// </summary>
        public TRight PeekRight { get; }

        /// <summary>
        /// Gets the Right value.
        /// Throws an exception if the Right value is not acceptable.
        /// </summary>
        /// <value>
        /// The right value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">Cannot access the Right value when the Right value is not acceptable.</exception>
        public TRight Right
            => IsRight
            ? PeekRight
            : throw new InvalidOperationException("Cannot access the Right value when not in the Right state.");

        /// <summary>
        /// Returns a value depending which combination of values are acceptable.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="ifLeft">If only the left value is acceptable.</param>
        /// <param name="ifRight">If only the right value is acceptable.</param>
        /// <param name="ifBoth">If both values are acceptable.</param>
        /// <param name="ifNeither">If neither value is acceptable.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// ifLeft
        /// or
        /// ifRight
        /// or
        /// ifBoth
        /// or
        /// ifNeither
        /// </exception>
        public TResult Return<TResult>(Func<TLeft, TResult> ifLeft, Func<TRight, TResult> ifRight, Func<TLeft, TRight, TResult> ifBoth, Func<TResult> ifNeither)
        {
            if (ifLeft == null) throw new ArgumentNullException(nameof(ifLeft));
            if (ifRight == null) throw new ArgumentNullException(nameof(ifRight));
            if (ifBoth == null) throw new ArgumentNullException(nameof(ifBoth));
            if (ifNeither == null) throw new ArgumentNullException(nameof(ifNeither));

            if (IsBoth)
                return ifBoth(this.Left, this.Right);
            else if (IsLeft)
                return ifLeft(this.Left);
            else if (IsRight)
                return ifRight(this.Right);
            else // Neither
                return ifNeither();
        }

        /// <summary>
        /// Returns a value depending which combination of values are acceptable.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="ifBoth">If both values are acceptable.</param>
        /// <param name="ifNeither">If neither value is acceptable.</param>
        /// <param name="otherwise">The value to return if not Both or Neither.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">ifLeft
        /// or
        /// ifRight
        /// or
        /// ifBoth
        /// or
        /// ifNeither</exception>
        public TResult Return<TResult>(Func<TLeft, TRight, TResult> ifBoth, Func<TResult> ifNeither, TResult otherwise)
        {
            if (ifBoth == null) throw new ArgumentNullException(nameof(ifBoth));
            if (ifNeither == null) throw new ArgumentNullException(nameof(ifNeither));

            if (IsBoth)
                return ifBoth(this.Left, this.Right);
            else if (IsNeither)
                return ifNeither();
            else
                return otherwise;
        }
    }
}