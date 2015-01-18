using System;
using EightPuzzle.Weights;

namespace EightPuzzle.Costs
{
    /// <summary>
    /// Class CostBase.
    /// </summary>
    abstract class CostBase : ICost
    {
        /// <summary>
        /// The weight
        /// </summary>
        protected readonly SimpleWeight Weight;

        /// <summary>
        /// The heuristic
        /// </summary>
        protected readonly IHeuristic Heuristic;

        /// <summary>
        /// Initializes a new instance of the <see cref="CostBase" /> class.
        /// </summary>
        /// <param name="weight">The weight.</param>
        /// <param name="heuristic">The heuristic.</param>
        /// <exception cref="ArgumentNullException">
        /// Weight was <see langword="null"/>
        /// or
        /// Heuristic was <see langword="null"/>
        /// </exception>
        protected CostBase(SimpleWeight weight, IHeuristic heuristic)
        {
            if (ReferenceEquals(weight, null)) throw new ArgumentNullException("weight", "Weight must not be null");
            if (ReferenceEquals(heuristic, null)) throw new ArgumentNullException("heuristic", "Heuristic must not be null");
            Weight = weight;
            Heuristic = heuristic;
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
        public abstract float DetermineCost(int[] prior, int[] posterior, Move move);
    }
}
