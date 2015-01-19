using System;

namespace EightPuzzle.Heuristics
{
    /// <summary>
    /// Class EuclideanDistanceHeuristic.
    /// <para>
    /// Calculates the sum of euclidean distances for each tile to move to its goal location.
    /// This equals the airline distance of each tile to the target destination.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This seems like a bad idea for the 8-Puzzle type problem since we
    /// can only move orthogonally, however it tends to create a much shorter solution to the problem.
    /// </remarks>
    sealed class EuclideanDistanceHeuristic : HeuristicBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanDistanceHeuristic" /> class.
        /// </summary>
        /// <param name="goalState">State goal state.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        public EuclideanDistanceHeuristic(int[] goalState, int width, int height)
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

            var sumOfDistances = 0F;
            for (var i = 0; i < count; ++i)
            {
                tileCoords.DetermineFrom(i);
                var block = posterior[i];

                // if the block is already at the required location,
                // skip to the next block (this adds 0 to the sum-of-distance, as expected)
                if (block == goal[i]) continue;

                // skip the empty place
                if (block == Program.EmptyFieldValue) continue;
                
                // since the block is not already correct,
                // let's find out where it should be.
                goalCoords.Reset();
                for (var targetIndex = 0; targetIndex < count; ++targetIndex)
                {
                    goalCoords.DetermineFrom(targetIndex);
                    if (goal[targetIndex] == block) break;
                }

                // the euclidean distance is the square root
                // of the sum of all squared component distances.
                // note that we omit the square root here,
                // since the condition
                // sqrt(a) > sqrt(b) <=> a > b
                // is always fulfilled.
                // Note that unless the square root is taken, the heuristic is NOT admissible.
                var xdistance = goalCoords.X - tileCoords.X;
                var ydistance = goalCoords.Y - tileCoords.Y;
                var distance = (float)xdistance * xdistance + (float)ydistance * ydistance;
                sumOfDistances += distance;
            }

            return sumOfDistances;
        }
    }
}
