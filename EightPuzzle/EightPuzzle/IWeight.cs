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
        /// <param name="move">The move.</param>
        /// <returns>The weight of this operation.</returns>
        float DetermineWeight(Move move);
    }
}
