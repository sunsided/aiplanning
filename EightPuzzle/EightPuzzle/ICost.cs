using System;

namespace EightPuzzle
{
    interface ICost
    {
        /// <summary>
        /// Determines the cost to perform the move, given the state.
        /// <para>
        /// This is the core of the search algorithm. A best-first search
        /// takes into account only the weight, greedy best-first only
        /// the heuristic and A* the sum of weight and heuristic.
        /// </para>
        /// </summary>
        /// <param name="prior">The a priori state.</param>
        /// <param name="posterior">The a posteriori state.</param>
        /// <param name="move">The move.</param>
        /// <returns>The cost of this operation.</returns>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        float DetermineCost(int[] prior, int[] posterior, Move move);
    }
}
