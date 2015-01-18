using System.Diagnostics;

namespace Search
{
    /// <summary>
    /// Struct Action
    /// </summary>
    [DebuggerDisplay("C{Cannibals}, M{Missionaries}")]
    struct Action
    {
        /// <summary>
        /// The number of cannibals in the boat
        /// </summary>
        public readonly int Cannibals;

        /// <summary>
        /// The number of missionaries in the boat
        /// </summary>
        public readonly int Missionaries;

        /// <summary>
        /// Initializes a new instance of the <see cref="Action"/> struct.
        /// </summary>
        /// <param name="cannibals">The cannibals.</param>
        /// <param name="missionaries">The missionaries.</param>
        public Action(int cannibals, int missionaries)
        {
            Cannibals = cannibals;
            Missionaries = missionaries;
        }
    }
}
