using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    class SearchNode
    {
        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <value>The state.</value>
        public State State { get; private set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public SearchNode Parent { get; private set; }

        /// <summary>
        /// Gets the causing action.
        /// </summary>
        /// <value>The causing action.</value>
        public Action CausingAction { get; private set; }

        /// <summary>
        /// Gets the cost.
        /// </summary>
        /// <value>The cost.</value>
        public int Cost { get; private set; }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>The depth.</value>
        public int Depth { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchNode"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="causingAction">The causing action.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="cost">The cost.</param>
        public SearchNode(State state, SearchNode parent, Action causingAction, int depth, int cost)
        {
            State = state;
            Parent = parent;
            CausingAction = causingAction;
            Depth = depth;
            Cost = cost;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SearchNode" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="SearchNode" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        protected bool Equals(SearchNode other)
        {
            return State.Equals(other.State); // && Equals(Parent, other.Parent);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SearchNode) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (State.GetHashCode()*397) ^ (Parent != null ? Parent.GetHashCode() : 0);
            }
        }
    }
}
