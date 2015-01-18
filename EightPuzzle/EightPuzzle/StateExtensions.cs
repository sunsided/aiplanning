using System;

namespace EightPuzzle
{
    /// <summary>
    /// Class StateExtensions.
    /// </summary>
    static class StateExtensions
    {
        /// <summary>
        /// Creates a posteriori state from the a priori state and the given move.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="move">The move.</param>
        /// <returns>System.Int32[].</returns>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        public static int[] CreateAPosteriori(this int[] state, Move move)
        {
            if (ReferenceEquals(state, null)) throw new ArgumentNullException("state", "State must not be null");

            // clone the a priori state
            var newState = (int[])state.Clone();

            // swap the pieces
            newState[move.To] = state[move.From];
            newState[move.From] = state[move.To];
            return newState;
        }
    }
}
