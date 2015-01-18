using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EightPuzzle.Costs;
using EightPuzzle.Heuristics;
using EightPuzzle.Weights;

// #define DumpIntermediateStates

namespace EightPuzzle
{
    class Program
    {
        private const int EmptyFieldValue = 0;

        static void Main()
        {
            const int puzzleWidth = 3;
            const int puzzleHeight = 3;

            int[] puzzle =
            {
                7, 2, 4,
                5, EmptyFieldValue, 6,
                8, 3, 1
            };

            puzzle = CreatePuzzle(width: puzzleWidth, height: puzzleHeight, seed: 0 /*1748953365*/);

            int[] goal =
            {
                EmptyFieldValue, 1, 2,
                3, 4, 5,
                6, 7, 8
            };

#if DumpIntermediateStates
            // dump the initial state
            Console.WriteLine("Initial state:");
            DumpState(puzzle, puzzleWidth);
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
                                   new Action(0, -1, default(Move), puzzle)
                               };

            // the fringe contains all nodes that are actively considered
            // for the solution path.
            var fringeComparer = new ActionByWeightComparer();

            // we start by initially filling the fringe with a start node.
            // in this case, there are be multiple.
            var fringe = DeterminePossibleActions(0, puzzle, puzzleWidth, costAlgorithm).ToList();

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
                var action = fringe.First();
                fringe.RemoveAt(0);

                // add the action to the visited nodes list
                visitedNodes.Add(action);
                var visitedNodeId = visitedNodes.Count - 1;

#if DumpIntermediateStates
                // dump the selected action
                Console.WriteLine("Selected state #{0}, parent #{1}:", visitedNodeId, action.VisitedNodeId);
                DumpState(action.State, puzzleWidth);
#endif

                // determine if this is a goal state
                if (IsSameState(action.State, goal))
                {
                    success = true;
                    break;
                }
                
                // expand next-generation states and add them to the fringe.
                var actions = DeterminePossibleActions(visitedNodeId, action.State, puzzleWidth, costAlgorithm);

                // we add the new states to the fringe, but take out
                // all actions that result in the same state we started with
                foreach (var next in actions)
                {
                    if (visitedNodes.Any(node => IsSameState(node.State, next.State)))
                    {
                        continue;
                    }
                    if (fringe.Any(node => IsSameState(node.State, next.State)))
                    {
                        continue;
                    }
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
                DumpSuccess(visitedNodes, puzzleWidth);
            }

            // wait for keypress
            if (Debugger.IsAttached) Console.ReadKey(true);
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
        /// <param name="state">The state.</param>
        /// <param name="width">The width.</param>
        /// <param name="costAlgorithm">The algorithm.</param>
        /// <returns>IEnumerable&lt;Action&gt;.</returns>
        /// <exception cref="ArgumentNullException">
        /// The state was <see langword="null"/>
        /// or
        /// the cost algorithm was <see langword="null"/>
        /// </exception>
        static IEnumerable<Action> DeterminePossibleActions(int visitedNodeId, int[] state, int width, ICost costAlgorithm)
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
                if (left >= rowStart && IsFreeSlotAt(state, left)) yield return CreateAction(visitedNodeId, state, i, left, costAlgorithm);
                if (right <= rowEnd && IsFreeSlotAt(state, right)) yield return CreateAction(visitedNodeId, state, i, right, costAlgorithm);
                if (up >= 0 && IsFreeSlotAt(state, up)) yield return CreateAction(visitedNodeId, state, i, up, costAlgorithm);
                if (down <= lastValidIndex && IsFreeSlotAt(state, down)) yield return CreateAction(visitedNodeId, state, i, down, costAlgorithm);
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
        /// <returns>Action.</returns>
        /// <exception cref="ArgumentNullException">The state was <see langword="null" />
        /// or
        /// the cost algorithm was <see langword="null" /></exception>
        static Action CreateAction(int visitedNodeId, int[] prior, int from, int to, ICost costAlgorithm)
        {
            if (ReferenceEquals(prior, null)) throw new ArgumentNullException("prior", "State must not be null");
            if (ReferenceEquals(costAlgorithm, null)) throw new ArgumentNullException("costAlgorithm", "The algorithm must not be null");

            // determine the a posteriori state from the move
            var move = new Move(from, to);
            var posterior = prior.CreateAPosteriori(move);

            // calculate the cost
            var cost = costAlgorithm.DetermineCost(prior, posterior, move);

            // bundle the new action
            return new Action(cost, visitedNodeId, move, posterior);
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
            if (index < 0) throw new ArgumentOutOfRangeException("index", "Index must be a positive number or zero");

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
        /// <param name="seed">The seed.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>System.Int32[].</returns>
        static int[] CreatePuzzle(int width, int height, int seed = 0)
        {
            if (seed == 0) seed = (int)DateTime.UtcNow.Ticks;
            Console.WriteLine("Creating {0}x{1} puzzle using seed {2}", width, height, seed);

            var random = new Random(seed);

            // prepare the list and add the empty value
            var count = width*height - 1;
            var list = new List<int> {EmptyFieldValue};
            Debug.Assert(EmptyFieldValue <= 0, "EmptyFieldValue <= 0");

            // create all valid tiles
            for (int i = 1; i <= count; ++i)
            {
                list.Add(i);
            }

            // shuffle the list
            var puzzle = list.OrderBy(value => random.NextDouble()).ToArray();
            return puzzle;
        }

        #region Console output

        /// <summary>
        /// Dumps the state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state">The state.</param>
        /// <param name="width">The width.</param>
        /// <exception cref="ArgumentNullException">state;State was null</exception>
        static void DumpState<T>(T state, int width) where T : IReadOnlyList<int>
        {
            if (ReferenceEquals(state, null)) throw new ArgumentNullException("state", "State was null");
            var count = state.Count;

            var indexInRow = -1;

            var sb = new StringBuilder();
            for (var i = 0; i < count; ++i)
            {
                // apply a linebreak on row changes
                if (++indexInRow == width)
                {
                    indexInRow = 0;
                    sb.AppendLine();
                }

                // fetch the value
                var value = state[i];

                // render the tile
                if (value != EmptyFieldValue)
                {
                    sb.Append(value);
                }
                else
                {
                    sb.Append(' ');
                }
            }

            // print out the puzzle to the console
            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Dumps the state.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="width">The width.</param>
        /// <exception cref="ArgumentNullException">state;State was null</exception>
        static void DumpAction(Action action, int width)
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
            }
            
            DumpState(state, width);
        }

        /// <summary>
        /// Dumps the success message
        /// </summary>
        /// <param name="visitedNodes">The visited nodes.</param>
        /// <param name="puzzleWidth">Width of the puzzle.</param>
        private static void DumpSuccess(IReadOnlyList<Action> visitedNodes, int puzzleWidth)
        {
            // unroll the solution, starting with the last added
            // action in the visited nodes list
            var steps = new Stack<Action>();
            var parent = visitedNodes.Count - 1;
            do
            {
                var action = visitedNodes[parent];
                steps.Push(action);
                parent = action.VisitedNodeId;
            } while (parent >= 0);

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
                DumpAction(action, puzzleWidth);
            }
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
