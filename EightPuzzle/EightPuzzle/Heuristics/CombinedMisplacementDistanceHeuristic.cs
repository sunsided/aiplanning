using System;

namespace EightPuzzle.Heuristics
{
    /// <summary>
    /// Class CombinedMisplacementDistanceHeuristic.
    /// <para>
    /// Combines the number of misplaced stones with a distance heuristic.
    /// </para>
    /// </summary>
    sealed class CombinedMisplacementDistanceHeuristic : HeuristicBase
    {
        /// <summary>
        /// The misplacement heuristic
        /// </summary>
        private readonly IHeuristic _misplacementHeuristic;

        /// <summary>
        /// The distance heuristic
        /// </summary>
        private readonly IHeuristic _distanceHeuristic;

        /// <summary>
        /// The base factor
        /// </summary>
        private float _baseFactor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedMisplacementDistanceHeuristic" /> class.
        /// </summary>
        /// <param name="goalState">State goal state.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        public CombinedMisplacementDistanceHeuristic(int[] goalState, int width, int height)
            : base(goalState, width, height)
        {
            _misplacementHeuristic = new TotalMisplacedTilesHeuristic(goalState, width, height);
            _distanceHeuristic = new EuclideanDistanceHeuristic(goalState, width, height);

            // the largest distance that can occur in the distance heuristic,
            // assuming that euclidean distance does not take the square root.
            var largestDistance = width*width + height*height;
            var largestHeuristic = largestDistance*(width*height);

            // determine the next-highest power of ten
            _baseFactor = (float)Math.Pow(10F, Math.Ceiling(Math.Log10(largestHeuristic)));
        }

        /// <summary>
        /// Determines the heuristic of the a posteriori state
        /// being a good solution.
        /// </summary>
        /// <param name="posterior">The a-posteriori state.</param>
        /// <param name="move">The move.</param>
        /// <returns>The cost of this operation.</returns>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        /// <exception cref="ArgumentException">Posterior state size must be equal to the goal state size</exception>
        public override float DetermineHeuristic(int[] posterior, Move move)
        {
            if (ReferenceEquals(posterior, null)) throw new ArgumentNullException("posterior", "State must not be null");
            ValidateAndThrow(posterior);

            // determine individual heuristics
            var misplacement = _misplacementHeuristic.DetermineHeuristic(posterior, move);
            var distance = _distanceHeuristic.DetermineHeuristic(posterior, move);

            // combine both measures
            return misplacement*_baseFactor + distance;
        }
    }
}
