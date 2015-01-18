using System;

namespace EightPuzzle
{
    /// <summary>
    /// Interface IWeight
    /// </summary>
    interface IWeight
    {
        /// <summary>
        /// Determines the weight of performing the move, i.e.
        /// the distance on the map.
        /// </summary>
        /// <param name="posterior">The posterior.</param>
        /// <param name="move">The move.</param>
        /// <returns>The weight of this operation.</returns>
        /// <exception cref="ArgumentNullException">posterior;State must not be null</exception>
        float DetermineWeight(int[] posterior, Move move);
    }
}
