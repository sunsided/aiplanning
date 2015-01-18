using System;

namespace EightPuzzle.Costs
{
    /// <summary>
    /// Class GreedyBestFirstCost. This class cannot be inherited.
    /// </summary>
    sealed class GreedyBestFirstCost : CostBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreedyBestFirstCost" /> class.
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="heuristic">The heuristic.</param>
        /// <exception cref="ArgumentNullException">
        /// Weight was <see langword="null"/>
        /// or
        /// Heuristic was <see langword="null"/>
        /// </exception>
        public GreedyBestFirstCost(IWeight weight, IHeuristic heuristic)
            : base(weight, heuristic)
        {
        }

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
        public override float DetermineCost(int[] prior, int[] posterior, Move move)
        {
            if (ReferenceEquals(prior, null)) throw new ArgumentNullException("prior", "State must not be null");
            if (ReferenceEquals(posterior, null)) throw new ArgumentNullException("posterior", "State must not be null");
            
            // Greedy best-first search determines it's path cost by only using the heuristic.
            var cost = Heuristic.DetermineHeuristic(posterior, move);
            return cost;
        }
    }
}
