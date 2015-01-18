using System;
using System.Diagnostics;

namespace EightPuzzle
{
    /// <summary>
    /// Struct MoveAction
    /// </summary>
    [DebuggerDisplay("{From} -> {To}")]
    struct Move : IEquatable<Move>
    {
        /// <summary>
        /// The previous index
        /// </summary>
        public readonly int From;

        /// <summary>
        /// The next index
        /// </summary>
        public readonly int To;

        /// <summary>
        /// Initializes a new instance of the <see cref="Move"/> struct.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public Move(int @from, int to)
        {
            From = @from;
            To = to;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return String.Format("{0} -> {1}", From, To);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Move" /> is equal to this instance.
        /// </summary>
        /// <param name="other">Another object to compare to.</param>
        /// <returns><see langword="true" /> if the specified <see cref="Move" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(Move other)
        {
            return From == other.From && To == other.To;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Move && Equals((Move)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (From * 397) ^ To;
            }
        }
    }
}
