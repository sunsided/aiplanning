using System;

namespace EightPuzzle
{
    /// <summary>
    /// Struct Action
    /// </summary>
    internal struct Action : IEquatable<Action>
    {
        /// <summary>
        /// The cost of the operation,
        /// including weight and heuristics.
        /// </summary>
        public readonly float Cost;

        /// <summary>
        /// The ID in the visited node buffer
        /// </summary>
        public readonly int VisitedNodeId;

        /// <summary>
        /// The associated move
        /// </summary>
        public readonly Move Move;

        /// <summary>
        /// The a-posteriori state
        /// </summary>
        public readonly int[] State;

        /// <summary>
        /// The depth in the search tree
        /// </summary>
        public readonly int Depth;

        /// <summary>
        /// Initializes a new instance of the <see cref="Action" /> struct.
        /// </summary>
        /// <param name="cost">The cost.</param>
        /// <param name="visitedNodeId">The visited node identifier.</param>
        /// <param name="move">The move.</param>
        /// <param name="state">The a-posteriori state.</param>
        /// <param name="depth">The depth.</param>
        public Action(float cost, int visitedNodeId, Move move, int[] state, int depth)
        {
            Cost = cost;
            VisitedNodeId = visitedNodeId;
            Move = move;
            State = state;
            Depth = depth;
        }

        #region Equality

        /// <summary>
        /// Determines whether the specified <see cref="Action" /> is equal to this instance.
        /// </summary>
        /// <param name="other">Another object to compare to.</param>
        /// <returns><see langword="true" /> if the specified <see cref="Action" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(Action other)
        {
            return Cost.Equals(other.Cost) && VisitedNodeId == other.VisitedNodeId && Move.Equals(other.Move) && Equals(State, other.State);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Action && Equals((Action) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Cost.GetHashCode();
                hashCode = (hashCode*397) ^ VisitedNodeId;
                hashCode = (hashCode*397) ^ Move.GetHashCode();
                hashCode = (hashCode*397) ^ (State != null ? State.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion Equality

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var from = Move.From;
            var to = Move.To;

            // verbally describe the action
            var description = GetActionDescription();

            // fromage!
            return String.Format("{0} from index {1} to {2}, cost {3}, parent {4}",
                description, from, to, Cost, VisitedNodeId);
        }

        /// <summary>
        /// Gets the action description.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetActionDescription()
        {
            var from = Move.From;
            var to = Move.To;

            // determine the direction
            var direction = "nowhere";
            if (to == from - 1) direction = "left";
            else if (to == from + 1) direction = "right";
            else if (to < from) direction = "up";
            else if (to > from) direction = "down";

            // determine the tile that was moved
            var movedTile = State[to];

            return String.Format("move [{0}] {1}", movedTile, direction);
        }
    }
}
