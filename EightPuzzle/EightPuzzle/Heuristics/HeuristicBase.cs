using System;

namespace EightPuzzle.Heuristics
{
    /// <summary>
    /// Class HeuristicBase.
    /// </summary>
    internal abstract class HeuristicBase : IHeuristic
    {
        /// <summary>
        /// The goal state
        /// </summary>
        protected readonly int[] GoalState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MisplacedTilesHeuristic" /> class.
        /// </summary>
        /// <param name="goalState">Goal state.</param>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        /// <exception cref="ArgumentException">Posterior state size must be equal to the goal state size</exception>
        protected HeuristicBase(int[] goalState)
        {
            if (ReferenceEquals(goalState, null)) throw new ArgumentNullException("goalState", "State must not be null");
            GoalState = goalState;
        }

        /// <summary>
        /// Determines the heuristic of the a posteriori state
        /// being a good solution.
        /// </summary>
        /// <param name="posterior">The a-posteriori state.</param>
        /// <param name="move">The move.</param>
        /// <returns>The cost of this operation.</returns>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        public abstract float DetermineHeuristic(int[] posterior, Move move);

        /// <summary>
        /// Validates the size.
        /// </summary>
        /// <param name="posterior">The posterior.</param>
        /// <exception cref="ArgumentException">Posterior state size must be equal to the goal state size</exception>
        protected void ValidateAndThrow(int[] posterior)
        {
            var count = GoalState.Length;
            if (posterior.Length != count) throw new ArgumentException("Posterior state size must be equal to the goal state size", "posterior");
        }
    }
}