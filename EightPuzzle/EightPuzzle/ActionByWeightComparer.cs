using System.Collections.Generic;

namespace EightPuzzle
{
    /// <summary>
    /// Class ActionComparer. This class cannot be inherited.
    /// </summary>
    sealed class ActionByWeightComparer : IComparer<Action>
    {
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
        public int Compare(Action x, Action y)
        {
            var result = x.Cost.CompareTo(y.Cost);

            // these are simply to keep a stable search order
            if (result == 0) result = x.VisitedNodeId.CompareTo(y.VisitedNodeId);
            if (result == 0) result = x.Move.From.CompareTo(y.Move.From);
            if (result == 0) result = x.Move.To.CompareTo(y.Move.To);

            return result;
        }
    }
}
