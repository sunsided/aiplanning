using System;

namespace EightPuzzle
{
    /// <summary>
    /// Interface IHeuristic
    /// </summary>
    interface IHeuristic
    {
        /// <summary>
        /// Determines the heuristic of the a posteriori state
        /// being a good solution.
        /// </summary>
        /// <param name="posterior">The a-posteriori state.</param>
        /// <param name="move">The move.</param>
        /// <returns>The cost of this operation.</returns>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        float DetermineHeuristic(int[] posterior, Move move);
    }
}
