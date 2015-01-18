using System;

namespace EightPuzzle.Heuristics
{
    /// <summary>
    /// Class MisplacedTilesHeuristic.
    /// </summary>
    sealed class MisplacedTilesHeuristic : HeuristicBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MisplacedTilesHeuristic"/> class.
        /// </summary>
        /// <param name="goalState">State goal state.</param>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        public MisplacedTilesHeuristic(int[] goalState)
            : base(goalState)
        {
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

            var goal = GoalState;
            var count = goal.Length;

            // count all mismatching tiles
            var mismatchingTiles = 0.0F;
            for (var i = 0; i < count; ++i)
            {
                // this approach will also count the misplacement
                // of the empty tile, but since this cancels out
                // when reaching the goal state, it will not be 
                // corrected for here.
                if (goal[i] != posterior[i]) ++mismatchingTiles;
            }

            return mismatchingTiles;
        }
    }
}
