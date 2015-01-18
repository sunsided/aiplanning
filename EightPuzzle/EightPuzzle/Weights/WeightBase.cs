using System;
using EightPuzzle.Heuristics;

namespace EightPuzzle.Weights
{
    /// <summary>
    /// Class WeightBase.
    /// </summary>
    internal abstract class WeightBase : IWeight
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
        protected WeightBase(int[] goalState, int width, int height)
        {
            if (ReferenceEquals(goalState, null)) throw new ArgumentNullException("goalState", "State must not be null");
            if (width <= 0) throw new ArgumentOutOfRangeException("width", "Width must be positive");
            if (height <= 0) throw new ArgumentOutOfRangeException("height", "Height must be positive");

            GoalState = goalState;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Determines the weight of performing the move, i.e.
        /// the distance on the map.
        /// </summary>
        /// <param name="posterior">The posterior.</param>
        /// <param name="move">The move.</param>
        /// <returns>The weight of this operation.</returns>
        /// <exception cref="ArgumentNullException">posterior;State must not be null</exception>
        public abstract float DetermineWeight(int[] posterior, Move move);

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