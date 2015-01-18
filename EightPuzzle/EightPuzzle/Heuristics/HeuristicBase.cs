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
        /// The width of the puzzle
        /// </summary>
        protected readonly int Width;
        
        /// <summary>
        /// The height of the puzzle
        /// </summary>
        protected readonly int Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalMisplacedTilesHeuristic" /> class.
        /// </summary>
        /// <param name="goalState">Goal state.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentNullException">State must not be null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Width must be positive
        /// and
        /// height must be positive</exception>
        /// <exception cref="ArgumentException">State must not be null</exception>
        protected HeuristicBase(int[] goalState, int width, int height)
        {
            if (ReferenceEquals(goalState, null)) throw new ArgumentNullException("goalState", "State must not be null");
            if (width <= 0) throw new ArgumentOutOfRangeException("width", "Width must be positive");
            if (height <= 0) throw new ArgumentOutOfRangeException("height", "Height must be positive");

            GoalState = goalState;
            Width = width;
            Height = height;
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