using System.Diagnostics;

namespace EightPuzzle
{
    /// <summary>
    /// Class CoordinateProjection. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("{X},{Y} at index {Index}")]
    sealed class CoordinateProjection
    {
        /// <summary>
        /// The width
        /// </summary>
        private readonly int _width;

        /// <summary>
        /// The height
        /// </summary>
        private readonly int _height;

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <value>The x coordinate.</value>
        public int X { get; private set; }

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <value>The y coordinate.</value>
        public int Y { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateProjection"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public CoordinateProjection(int width, int height)
        {
            _width = width;
            _height = height;
            Reset();
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            Y = 0;
            X = 0;
            Index = 0;
        }

        /// <summary>
        /// Increments the index.
        /// </summary>
        public void IncrementIndex()
        {
            ++Index;
            if (++X == _width)
            {
                X = 0;
                ++Y;
            }
        }

        /// <summary>
        /// Unrolls the specified index to a coordinate.
        /// </summary>
        /// <param name="index">The index.</param>
        public void DetermineFrom(int index)
        {
            // erliest index
            if (index == Index) return;

            // quite early exit
            if (index == Index + 1)
            {
                IncrementIndex();
                return;
            }

            // there was a jump, so do it the hard way
            Index = index;
            X = index % _height;
            Y = index / _width;
        }
    }
}
