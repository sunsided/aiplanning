using System.Diagnostics;

namespace Search
{
    /// <summary>
    /// Struct State
    /// </summary>
    [DebuggerDisplay("Left: C{CannibalsLeft}, M{MissionariesLeft}; Right: C{CannibalsRight}, M{MissionariesRight}; Boat: {BoatLocation}")]
    struct State
    {
        /// <summary>
        /// The number of cannibals on the left side
        /// </summary>
        public readonly int CannibalsLeft;

        /// <summary>
        /// The cannibals on the right side
        /// </summary>
        public readonly int CannibalsRight;

        /// <summary>
        /// The missionaries on the left side
        /// </summary>
        public readonly int MissionariesLeft;

        /// <summary>
        /// The missionaries on the right side
        /// </summary>
        public readonly int MissionariesRight;

        /// <summary>
        /// The boat location
        /// </summary>
        public readonly BoatLocation BoatLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> struct.
        /// </summary>
        /// <param name="cannibalsLeft">The cannibals on the left.</param>
        /// <param name="cannibalsRight">The cannibals on the right.</param>
        /// <param name="missionariesLeft">The missionaries on the left.</param>
        /// <param name="missionariesRight">The missionaries on the right.</param>
        /// <param name="boatLocation">The boat location.</param>
        public State(int cannibalsLeft, int cannibalsRight, int missionariesLeft, int missionariesRight, BoatLocation boatLocation)
        {
            CannibalsLeft = cannibalsLeft;
            CannibalsRight = cannibalsRight;
            MissionariesLeft = missionariesLeft;
            MissionariesRight = missionariesRight;
            BoatLocation = boatLocation;
        }

        /// <summary>
        /// Determines whether the specified <see cref="State" /> is equal to this instance.
        /// </summary>
        /// <param name="other">Another object to compare to.</param>
        /// <returns><c>true</c> if the specified <see cref="State" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(State other)
        {
            return CannibalsLeft == other.CannibalsLeft && CannibalsRight == other.CannibalsRight && MissionariesLeft == other.MissionariesLeft && MissionariesRight == other.MissionariesRight && BoatLocation == other.BoatLocation;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is State && Equals((State) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CannibalsLeft;
                hashCode = (hashCode*397) ^ CannibalsRight;
                hashCode = (hashCode*397) ^ MissionariesLeft;
                hashCode = (hashCode*397) ^ MissionariesRight;
                hashCode = (hashCode*397) ^ (int) BoatLocation;
                return hashCode;
            }
        }
    }
}
