namespace EightPuzzle.Weights
{
    /// <summary>
    /// Class SimpleWeight. This class cannot be inherited.
    /// </summary>
    sealed class SimpleWeight : WeightBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceWeight" /> class.
        /// </summary>
        /// <param name="goalState">Goal state.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public SimpleWeight(int[] goalState, int width, int height) 
            : base(goalState, width, height)
        {
        }

        /// <summary>
        /// Determines the weight of performing the move, i.e.
        /// the distance on the map.
        /// </summary>
        /// <param name="unused">The unused posterior.</param>
        /// <param name="move">The move.</param>
        /// <returns>The weight of this operation.</returns>
        public override float DetermineWeight(int[] unused, Move move)
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
