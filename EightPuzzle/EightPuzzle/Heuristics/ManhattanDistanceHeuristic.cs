using System;

namespace EightPuzzle.Heuristics
{
    /// <summary>
    /// Class ManhattanHeuristic.
    /// <para>
    /// Calculates the sum of manhattan distances for each tile to move to its goal location.
    /// This equals the smallest number of moves required for each tile to reach the target destination.
    /// </para>
    /// </summary>
    sealed class ManhattanDistanceHeuristic : HeuristicBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManhattanDistanceHeuristic" /> class.
        /// </summary>
        /// <param name="goalState">State goal state.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        public ManhattanDistanceHeuristic(int[] goalState, int width, int height)
            : base(goalState, width, height)
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
            var tileCoords = new CoordinateProjection(Width, Height);
            var goalCoords = new CoordinateProjection(Width, Height);

            var sumOfDistances = 0;
            for (var i = 0; i < count; ++i)
            {
                tileCoords.DetermineFrom(i);
                var block = posterior[i];

                // if the block is already at the required location,
                // skip to the next block (this adds 0 to the sum-of-distance, as expected)
                if (block == goal[i]) continue;
                
                // since the block is not already correct,
                // let's find out where it should be.
                goalCoords.Reset();
                for (var targetIndex = 0; targetIndex < count; ++targetIndex)
                {
                    goalCoords.DetermineFrom(targetIndex);
                    if (goal[targetIndex] == block) break;
                }

                // the manhattan distance is now the sum of all
                // absolute component distances.
                var distance = Math.Abs(goalCoords.X - tileCoords.X) + Math.Abs(goalCoords.Y - tileCoords.Y);
                sumOfDistances += distance;
            }

            return sumOfDistances;
        }
    }
}
