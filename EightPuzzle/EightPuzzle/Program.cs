// uncomment to print out intermediate steps
// #define DumpIntermediateStates

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EightPuzzle.Costs;
using EightPuzzle.Heuristics;
using EightPuzzle.Weights;

namespace EightPuzzle
{
    /// <summary>
    /// Class Program.
    /// </summary>
    static class Program
    {
        public const int EmptyFieldValue = 0;

        static void Main()
        {
            Console.Title = "8 Puzzle Solver";

#if !GeneratePuzzle
            const int puzzleWidth = 3;
            const int puzzleHeight = 3;

            int[] puzzle =
            {
                8, 1, 7,
                4, 5, 6,
                2, EmptyFieldValue, 3
            };

            int[] goal =
            {
                EmptyFieldValue, 1, 2,
                3, 4, 5,
                6, 7, 8
            };
#else
            const int puzzleWidth = 3;
            const int puzzleHeight = 3;

            int[] goal;
            var puzzle = CreatePuzzle(width: puzzleWidth, height: puzzleHeight, goal: out goal, seed: 0);
#endif

#if DumpIntermediateStates
            // dump the initial state
            Console.WriteLine("Initial state:");
            DumpState(puzzle, puzzleWidth, puzzleHeight);
#endif

            // select a weight calculation
            var weight = new SimpleWeight(goal, puzzleWidth, puzzleHeight);

            // select an heuristic
            var heuristic = new EuclideanDistanceHeuristic(goal, puzzleWidth, puzzleHeight);

            // select the algorithm
            var costAlgorithm = new AStarCost(weight, heuristic);

            // the visited nodes buffer helps us keep track of
            // nodes that have been expanded
            var visitedNodes = new List<Action>
                               {
                                   // add the initial state
                                   new Action(0, -1, default(Move), puzzle, 0)
                               };

            // the fringe contains all nodes that are actively considered
            // for the solution path.
            var fringeComparer = new ActionByWeightComparer();

            // we start by initially filling the fringe with a start node.
            // in this case, there are be multiple.
            var fringe = DeterminePossibleActions(0, 0, puzzle, puzzleWidth, costAlgorithm, 0F).ToList();

            // map that helps counting the total number of nodes in each tree depth
            // (maps tree depth to number)
            var counter = new ConcurrentDictionary<int, int>();
            counter.TryAdd(1, fringe.Count);

            // now we sort the fringe in order to keep a priority queue
            fringe.Sort(fringeComparer);

            // iterate to the solution
            var success = false;
            while (true)
            {
                // if the fringe is empty, we failed
                if (fringe.Count == 0)
                {
                    break;
                }

                // take the first item from the fringe
                var action = TakeFromFringe(fringe);

                // add the action to the visited nodes list
                visitedNodes.Add(action);
                var visitedNodeId = visitedNodes.Count - 1;

#if DumpIntermediateStates
                // dump the selected action
                Console.WriteLine("Selected state #{0}, parent #{1}:", visitedNodeId, action.VisitedNodeId);
                DumpState(action.State, puzzleWidth, puzzleHeight, action.Move.To);
#endif

                // determine if this is a goal state
                if (IsSameState(action.State, goal))
                {
                    success = true;
                    break;
                }
                
                // expand next-generation states and add them to the fringe.
                var actions = DeterminePossibleActions(visitedNodeId, action.Depth, action.State, puzzleWidth, costAlgorithm, action.Cost);

                // fetch the parent
                var parent = visitedNodes[action.VisitedNodeId];

                // we add the new states to the fringe, but take out
                // all actions that result in the same states we already tested.
                // the rationale is that all a posteriori states have been 
                // in the fringe, so are not required to be tested again.
                foreach (var next in actions)
                {
                    counter.AddOrUpdate(next.Depth, key => 1, (key, value) => value + 1);

                    // prevent deadlocks by disallowing undoing the previous operation
                    // if (IsSameState(next.State, parent.State)) continue;

                    // test if the state has already been seen.
                    // if so, discard the expanded node only if its cost is higher than the
                    // cost of the already-registered node.
                    // this allows us to keep shortcuts, if found.
                    if (WasAlreadyAnticipated(visitedNodes, next)) continue;

                    // skip elements that are already in the fringe
                    // if (IsElementInFringe(fringe, next)) continue;

                    fringe.Add(next);
                }

                // the fringe is sorted again to keep the lowest-cost items in the front
                fringe.Sort(fringeComparer);
            }

            // dump solution
            if (!success)
            {
                DumpFailure(visitedNodes);
            }
            else
            {
                DumpSuccess(visitedNodes, puzzleWidth, puzzleHeight);
            }

            // wait for keypress
            if (Debugger.IsAttached) Console.ReadKey(true);
        }

        /// <summary>
        /// Determines whether the specified element was already anticipated for expansion.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitedNodes">The visited nodes.</param>
        /// <param name="next">The next.</param>
        /// <returns><see langword="true" /> if the specified element was already anticipated; otherwise, <see langword="false" />.</returns>
        private static bool WasAlreadyAnticipated<T>(T visitedNodes, Action next) where T : IReadOnlyList<Action>
        {
            for (var f = visitedNodes.Count-1; f >= 0; --f)
            {
                if (!IsSameState(visitedNodes[f].State, next.State)) continue;
                if (visitedNodes[f].Cost > next.Cost)
                {
                    Debugger.Break();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified element is already in the fringe
        /// </summary>
        /// <param name="fringe">The fringe.</param>
        /// <param name="next">The next.</param>
        /// <returns><see langword="true" /> if the specified element is already in the fringe; otherwise, <see langword="false" />.</returns>
        private static bool IsElementInFringe<T>(T fringe, Action next) where T : IReadOnlyList<Action>
        {
            for (var f = 0; f < fringe.Count; ++f)
            {
                if (!IsSameState(fringe[f].State, next.State)) continue;
                if (fringe[f].Cost > next.Cost)
                {
                    Debugger.Break();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// The randomizer for the fringe
        /// </summary>
        private static readonly Random _fringeRandom = new Random();

        /// <summary>
        /// Takes an item from fringe.
        /// <para>
        /// This implementation uses a Las Vegas algorithm in order
        /// to (sometimes) speed up tree traversal.
        /// </para>
        /// </summary>
        /// <param name="fringe">The fringe.</param>
        /// <returns>Action.</returns>
        private static Action TakeFromFringe<T>(T fringe) where T : IList<Action>
        {
            var first = fringe.First();
            var count = fringe.Count;

            // determine the last index with the same cost
            int lastIndexWithSameCost;
            for (lastIndexWithSameCost = 0; lastIndexWithSameCost < count-1; ++lastIndexWithSameCost)
            {
                if (fringe[lastIndexWithSameCost + 1].Cost > first.Cost) break;
            }

            // randomly select an instance from the range
            var selectedIndex = _fringeRandom.Next(0, lastIndexWithSameCost + 1);
            var selected = fringe[selectedIndex];
            fringe.RemoveAt(selectedIndex);
            return selected;
        }

        /// <summary>
        /// Determines whether the specified state is a goal state.
        /// </summary>
        /// <param name="a">The state.</param>
        /// <param name="b">The goal.</param>
        /// <returns><see langword="true" /> if the specified state is a goal state; otherwise, <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="a"/> or <paramref name="b"/> was null
        /// </exception>
        static bool IsSameState<T>(T a, T b) where T : IReadOnlyList<int>
        {
            if (ReferenceEquals(a, null)) throw new ArgumentNullException("a", "State was null");
            if (ReferenceEquals(b, null)) throw new ArgumentNullException("b", "State was null");
            Debug.Assert(a.Count == b.Count, "a.Length == b.Length");

            var count = a.Count;
            for (var i = 0; i < count; ++i)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines the possible actions from the current state.
        /// </summary>
        /// <param name="visitedNodeId">The visited node identifier.</param>
        /// <param name="parentDepth">The parent depth.</param>
        /// <param name="state">The state.</param>
        /// <param name="width">The width.</param>
        /// <param name="costAlgorithm">The algorithm.</param>
        /// <param name="cumulativeCost">The cumulative cost.</param>
        /// <returns>IEnumerable&lt;Action&gt;.</returns>
        /// <exception cref="ArgumentNullException">The state was <see langword="null" />
        /// or
        /// the cost algorithm was <see langword="null" /></exception>
        static IEnumerable<Action> DeterminePossibleActions(int visitedNodeId, int parentDepth, int[] state, int width, ICost costAlgorithm, float cumulativeCost)
        {
            if (ReferenceEquals(state, null)) throw new ArgumentNullException("state", "State must not be null");
            if (ReferenceEquals(costAlgorithm, null)) throw new ArgumentNullException("costAlgorithm", "The algorithm must not be null");

            // prepare / fetch constants
            var count = state.Length;
            var indexInRow = -1;
            var rowStart = 0;
            var rowEnd = width - 1;
            var lastValidIndex = count - 1;

            for (var i = 0; i < count; ++i)
            {
                // track the rows and indices within the rows
                if (++indexInRow >= width)
                {
                    indexInRow = 0;
                    rowStart = i;
                    rowEnd = i + width - 1;
                }

                // skip the empty field
                if (state[i] == EmptyFieldValue) continue;

                // determine the adjacent states
                var left = i - 1;
                var right = i + 1;
                var up = i - width;
                var down = i + width;

                // check if the moves are possible
                var depth = parentDepth + 1;
                if (left >= rowStart && IsFreeSlotAt(state, left)) yield return CreateAction(visitedNodeId, state, i, left, costAlgorithm, depth, cumulativeCost);
                if (right <= rowEnd && IsFreeSlotAt(state, right)) yield return CreateAction(visitedNodeId, state, i, right, costAlgorithm, depth, cumulativeCost);
                if (up >= 0 && IsFreeSlotAt(state, up)) yield return CreateAction(visitedNodeId, state, i, up, costAlgorithm, depth, cumulativeCost);
                if (down <= lastValidIndex && IsFreeSlotAt(state, down)) yield return CreateAction(visitedNodeId, state, i, down, costAlgorithm, depth, cumulativeCost);
            }
        }

        /// <summary>
        /// Creates the actual action / applies the state transition.
        /// </summary>
        /// <param name="visitedNodeId">The visited node identifier.</param>
        /// <param name="prior">The a priori state.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="costAlgorithm">The algorithm.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="cumulativeCost">The cumulative cost.</param>
        /// <returns>Action.</returns>
        /// <exception cref="ArgumentNullException">The state was <see langword="null" />
        /// or
        /// the cost algorithm was <see langword="null" /></exception>
        static Action CreateAction(int visitedNodeId, int[] prior, int from, int to, ICost costAlgorithm, int depth, float cumulativeCost)
        {
            if (ReferenceEquals(prior, null)) throw new ArgumentNullException("prior", "State must not be null");
            if (ReferenceEquals(costAlgorithm, null)) throw new ArgumentNullException("costAlgorithm", "The algorithm must not be null");

            // determine the a posteriori state from the move
            var move = new Move(from, to);
            var posterior = prior.CreateAPosteriori(move);

            // calculate the cost
            var cost = cumulativeCost + costAlgorithm.DetermineCost(prior, posterior, move);

            // bundle the new action
            return new Action(cost, visitedNodeId, move, posterior, depth);
        }

        /// <summary>
        /// Determines whether there is a free slot at the specified index, given the state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="index">The index.</param>
        /// <returns><see langword="true" /> if there is a free slot at the specified index; otherwise, <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">state;State must not be null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Attempted to access an invalid index within the state</exception>
        static bool IsFreeSlotAt<T>(T state, int index) where T : IReadOnlyList<int>
        {
            if (ReferenceEquals(state, null)) throw new ArgumentNullException("state", "State must not be null");
            if (index < 0 || index >= state.Count) throw new ArgumentOutOfRangeException("index", "Index must be a positive number or zero and smaller than the number of elements");

            try
            {
                return state[index] == EmptyFieldValue;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException("Attempted to access invalid index in state at " + index, ex);
            }
        }

        /// <summary>
        /// Creates a new puzzle.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="goal">The goal.</param>
        /// <param name="seed">The seed.</param>
        /// <returns>System.Int32[].</returns>
        static int[] CreatePuzzle(int width, int height, out int[] goal, int seed = 0)
        {
            if (seed == 0) seed = (int)DateTime.UtcNow.Ticks;
            Console.WriteLine("Creating {0}x{1} puzzle using seed {2}", width, height, seed);

            var random = new Random(seed);

            // prepare the list and add the empty value
            var count = width*height - 1;
            var list = new List<int>();
            Debug.Assert(EmptyFieldValue <= 0, "EmptyFieldValue <= 0");

            // create all valid tiles
            for (int i = 1; i <= count; ++i)
            {
                list.Add(i);
            }

            // in order for the implemented solvability test to work,
            // the empty field is required to be on the last index
            list.Add(EmptyFieldValue);

            // export it as a goal
            goal = list.ToArray();

            // shuffle the list until the puzzle is solvable.
            // this is a brute-force attempt; it would probably be easier
            // to just perform random (valid) moves for a given amount
            // of iterations.
            int[] puzzle;
            do
            {
                puzzle = list.OrderBy(value => random.NextDouble()).ToArray();
            } while (!IsSolvable(puzzle, width, height));

            DumpState(puzzle, width, height);

            return puzzle;
        }

        /// <summary>
        /// Determines whether the specified puzzle is solvable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="puzzle">The puzzle.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns><see langword="true" /> if the specified puzzle is solvable; otherwise, <see langword="false" />.</returns>
        static bool IsSolvable<T>(T puzzle, int width, int height) where T : IReadOnlyList<int>
        {
            // see:  http://www.cs.bham.ac.uk/~mdr/teaching/modules04/java2/TilesSolvability.html
            // also: http://www.cs.princeton.edu/courses/archive/fall12/cos226/assignments/8puzzle.html
            var inversions = DetermineInversions(puzzle);

            // if the grid width is even, the solvability of
            // the puzzle depends on the position of the empty place
            if (IsEven(width))
            {
                // determine the position of the empty tile
                var count = puzzle.Count;
                var coords = new CoordinateProjection(width, height);
                for (int i = 0; i < count; ++i)
                {
                    coords.DetermineFrom(i);
                    if (puzzle[i] == EmptyFieldValue) break;
                }

                // if the empty tile is in an even row counting from the bottom
                var rowFromBottom = height - coords.Y;
                if (IsEven(rowFromBottom))
                {
                    // ... then the number of inversions must be odd
                    return !IsEven(inversions);
                }

                // ... otherwise the number of inversions must be even
                return IsEven(inversions);
            }

            // if the grid width is odd, then the number of inversions
            // must be even to be able to solve the puzzle
            return IsEven(inversions);
        }

        /// <summary>
        /// Determines whether the specified value is even.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><see langword="true" /> if the specified value is even; otherwise, <see langword="false" />.</returns>
        static bool IsEven(int value)
        {
            // mask with 0x01 in order to check 
            // if the value is even or odd
            var mask = value & 0x01;
            if (mask == 0) return true;
            return false;
        }

        /// <summary>
        /// Determines the inversions of the puzzle in order
        /// to check solvability.
        /// </summary>
        /// <param name="puzzle">The puzzle.</param>
        /// <returns>The number of inversions</returns>
        static int DetermineInversions<T>(T puzzle) where T : IReadOnlyList<int>
        {
            var count = puzzle.Count;
            var inversions = 0;
            for (int i = 0; i < count; ++i)
            {
                var value = puzzle[i];
                if (value == EmptyFieldValue) continue;

                for (int j = i + 1; j < count; ++j)
                {
                    var comparand = puzzle[j];
                    if (comparand == EmptyFieldValue) continue;
                    if (value > puzzle[j]) ++inversions;
                }
            }

            return inversions;
        }

        #region Console output

        /// <summary>
        /// Dumps the state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state">The state.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="to">The index, the move was made to.</param>
        /// <exception cref="ArgumentNullException">state;State was null</exception>
        static void DumpState<T>(T state, int width, int height, int to = -1) where T : IReadOnlyList<int>
        {
            if (ReferenceEquals(state, null)) throw new ArgumentNullException("state", "State was null");
            var count = state.Count;

            var indexInRow = -1;
            var evenWidth = IsEven(width);

            // determine the largest number; one is subtracted
            // because of the empty tile
            var largestNumber = width * height - 1;
            var blockSize = largestNumber.ToString().Length;
            var format = String.Format("{{0,{0}}}", blockSize+1);

            for (var i = 0; i < count; ++i)
            {
                // apply a linebreak on row changes
                if (++indexInRow == width)
                {
                    indexInRow = 0;
                    Console.ResetColor();
                    Console.WriteLine();
                }

                // fetch the value
                var value = state[i];

                // render the tile
                if (value != EmptyFieldValue)
                {
                    var valueEven = IsEven(value);
                    bool toggle;
                    var rowEven = IsEven((value-1)/width);
                    if (evenWidth)
                    {
                        toggle = (!rowEven && !valueEven) || (rowEven && valueEven);
                    }
                    else
                    {
                        toggle = valueEven;
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = toggle ? ConsoleColor.DarkBlue : ConsoleColor.DarkMagenta;

                    // if the 'to' index is given, highlight the element
                    if (i == to) Console.BackgroundColor = ConsoleColor.DarkGreen;

                    Console.Write(format, value);
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;

                    // if the 'to' index is given, highlight the element
                    if (to >= 0) Console.BackgroundColor = ConsoleColor.DarkGreen;

                    Console.Write(format, "");
                }
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Dumps the state.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="highlightMove">if set to <see langword="true" /> [highlight move].</param>
        /// <exception cref="ArgumentNullException">state;State was null</exception>
        static void DumpAction(Action action, int width, int height, bool highlightMove = false)
        {
            var state = action.State;

            if (action.VisitedNodeId >= 0)
            {
                var description = action.GetActionDescription();
                Console.WriteLine(description);
            }
            else
            {
                Console.WriteLine("Initial state");
                highlightMove = false;
            }

            DumpState(state, width, height, highlightMove ? action.Move.To : -1);
        }

        /// <summary>
        /// Dumps the success message
        /// </summary>
        /// <param name="visitedNodes">The visited nodes.</param>
        /// <param name="puzzleWidth">Width of the puzzle.</param>
        /// <param name="puzzleHeight">Height of the puzzle.</param>
        private static void DumpSuccess(IReadOnlyList<Action> visitedNodes, int puzzleWidth, int puzzleHeight)
        {
            // unroll the solution, starting with the last added
            // action in the visited nodes list
            var steps = new Stack<Action>();
            var parent = visitedNodes.Count - 1;
            var finalState = visitedNodes[parent];
            do
            {
                var action = visitedNodes[parent];
                steps.Push(action);
                parent = action.VisitedNodeId;
            } while (parent >= 0);

            // prepare the console by reserving enough buffer lines to fit in the whole solution.
            // values are purely empirical.
            var bufferLines = (steps.Count + 1)*(puzzleHeight + 1) + 20;
            Console.BufferHeight = Math.Max(Console.BufferHeight, bufferLines);

            // print the header
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Solution found in {0} steps, length {1} steps.", visitedNodes.Count, steps.Count);
            Console.ResetColor();

            // dump the solution
            int stepNumber = 0;
            while (steps.Count > 0)
            {
                if (stepNumber++ > 0)
                {
                    Console.Write("Step {0}: ", stepNumber);
                }

                var action = steps.Pop();
                DumpAction(action, puzzleWidth, puzzleHeight, highlightMove: true);
            }
                
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Final state:");
            Console.ResetColor();
            DumpState(finalState.State, puzzleWidth, puzzleHeight);
        }

        /// <summary>
        /// Dumps the failure message.
        /// </summary>
        /// <param name="visitedNodes">The visited nodes.</param>
        private static void DumpFailure(IReadOnlyCollection<Action> visitedNodes)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No solution found after {0} steps.", visitedNodes.Count);
            Console.ResetColor();
        }

        #endregion Console output
    }
}
