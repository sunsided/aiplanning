namespace EightPuzzle.Weights
{
    /// <summary>
    /// Class SimpleWeight. This class cannot be inherited.
    /// </summary>
    sealed class SimpleWeight : IWeight
    {
        /// <summary>
        /// Determines the weight of performing the move, i.e.
        /// the distance on the map.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <returns>The weight of this operation.</returns>
        public float DetermineWeight(Move move)
        {
            if (move.From == move.To) return 0;

            // every move has a unit distance.
            // this does not allow jumping over tiles, but since
            // such kind of move isn't allowed in the first place,
            // it's not checked for here.
            return 1;
        }
    }
}
