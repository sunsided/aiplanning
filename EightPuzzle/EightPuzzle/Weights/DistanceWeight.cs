using System;
using EightPuzzle.Heuristics;

namespace EightPuzzle.Weights
{
    /// <summary>
    /// Class DistanceWeight. This class cannot be inherited.
    /// <para>
    /// This class essentially uses a distance heuristic to determine the path weights.
    /// This way, moves away from the goal location become more costly. However, such approach
    /// directly undermines the idea of A* as both heuristic and weight apply the
    /// very same measure, effectively turning this into a greedy best-first search.
    /// The code is kept here for the sake of experimentation though.
    /// </para>
    /// </summary>
    sealed class DistanceWeight : WeightBase
    {
        /// <summary>
        /// The heuristic
        /// </summary>
        private readonly IHeuristic _heuristic;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceWeight" /> class.
        /// </summary>
        /// <param name="goalState">Goal state.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public DistanceWeight(int[] goalState, int width, int height) 
            : base(goalState, width, height)
        {
            _heuristic = new ManhattanDistanceHeuristic(goalState, width, height);
        }

        /// <summary>
        /// Determines the weight of performing the move, i.e.
        /// the distance on the map.
        /// </summary>
        /// <param name="posterior">The posterior.</param>
        /// <param name="move">The move.</param>
        /// <returns>The weight of this operation.</returns>
        /// <exception cref="ArgumentNullException">posterior;State must not be null</exception>
        public override float DetermineWeight(int[] posterior, Move move)
        {
            if (ReferenceEquals(posterior, null)) throw new ArgumentNullException("posterior", "State must not be null");
            ValidateAndThrow(posterior);

            if (move.From == move.To) return 0;
            return _heuristic.DetermineHeuristic(posterior, move);
        }
    }
}
