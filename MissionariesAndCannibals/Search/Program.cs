using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    class Program
    {
        static void Main(string[] args)
        {
            // the goal determination function
            Func<State, bool> isGoalState =
                state =>
                    state.CannibalsRight == 3 && state.MissionariesRight == 3 &&
                    state.BoatLocation == BoatLocation.Right;

            // create the initial state
            var initialState = new State(3, 0, 3, 0, BoatLocation.Left);

            // initiate the tree search
            var fringe = new Queue<SearchNode>();
            fringe.Enqueue(new SearchNode(initialState, null, default(Action), 0, 0));

            // keep track of anticipated states
            var anticipatedStates = new HashSet<State> {initialState};

            // search the tree
            while (true)
            {
                if (fringe.Count == 0)
                {
                    Console.WriteLine("No solution found.");
                    Console.ReadKey(true);
                    return;
                }

                // select a node from the fringe
                var node = SelectNode(fringe);

                // check for goal condition
                if (isGoalState(node.State))
                {
                    Console.WriteLine("Found solution");

                    // determine stack trace
                    var stack = new Stack<SearchNode>();
                    while (node != null)
                    {
                        stack.Push(node);
                        node = node.Parent;
                    }

                    // print trace, but skip the root node
                    foreach (var trace in stack.Skip(1))
                    {
                        Console.WriteLine(
                            "L:C{7}M{8}, R:C{9},M{10}, B:{11} \t--> A:C{5}M{6} --> \tL:C{0}M{1}, R:C{2},M{3}, B:{4}",
                            trace.State.CannibalsLeft,
                            trace.State.MissionariesLeft,
                            trace.State.CannibalsRight,
                            trace.State.MissionariesRight,
                            trace.State.BoatLocation,
                            trace.CausingAction.Cannibals,
                            trace.CausingAction.Missionaries,
                            trace.Parent.State.CannibalsLeft,
                            trace.Parent.State.MissionariesLeft,
                            trace.Parent.State.CannibalsRight,
                            trace.Parent.State.MissionariesRight,
                            trace.Parent.State.BoatLocation
                            );
                    }
                    
                    Console.ReadKey(true);
                    return;
                }

                // apply strategy and add to fringe
                PerformStrategy(fringe, node, anticipatedStates);
            }
        }

        /// <summary>
        /// The set of possible actions
        /// </summary>
        private static readonly Action[] PossibleActions = {
                new Action(2, 0), // move two cannibals
                new Action(1, 0), // move one cannibal
                new Action(0, 2), // move two missionaries
                new Action(0, 1), // move one missionary
                new Action(1, 1)  // move one cannibal and one missionary
            };

        /// <summary>
        /// Performs the strategy.
        /// </summary>
        /// <param name="fringe">The fringe.</param>
        /// <param name="node">The node.</param>
        private static void PerformStrategy(Queue<SearchNode> fringe, SearchNode node, HashSet<State> anticipatedStates)
        {
            var state = node.State;
            
            foreach (var action in PossibleActions)
            {
                State newState;

                // apply the state transition function
                if (state.BoatLocation == BoatLocation.Left)
                {
                    newState = new State(
                        state.CannibalsLeft - action.Cannibals,
                        state.CannibalsRight + action.Cannibals,
                        state.MissionariesLeft - action.Missionaries,
                        state.MissionariesRight + action.Missionaries,
                        BoatLocation.Right);
                }
                else
                {
                    newState = new State(
                        state.CannibalsLeft + action.Cannibals,
                        state.CannibalsRight - action.Cannibals,
                        state.MissionariesLeft + action.Missionaries,
                        state.MissionariesRight - action.Missionaries,
                        BoatLocation.Left);
                }

                // check if the action can be performed
                if (newState.CannibalsLeft < 0) continue;
                if (newState.CannibalsLeft > 3) continue;
                if (newState.CannibalsRight < 0) continue;
                if (newState.CannibalsRight > 3) continue;
                if (newState.MissionariesLeft < 0) continue;
                if (newState.MissionariesLeft > 3) continue;
                if (newState.MissionariesRight < 0) continue;
                if (newState.MissionariesRight > 3) continue;

                // check if the state is actually valid
                if (!IsValidState(newState)) continue;
                    
                // check if we have already anticipated this state
                if (anticipatedStates.Contains(newState)) continue;
                anticipatedStates.Add(newState);

                // add the new state to the fringe
                fringe.Enqueue(new SearchNode(newState, node, action, node.Depth + 1, 1));
            }
        }

        /// <summary>
        /// Determines whether the given state is valid.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns><c>true</c> if the given state is valid; otherwise, <c>false</c>.</returns>
        private static bool IsValidState(State state)
        {
            if (state.CannibalsLeft > state.MissionariesLeft && state.MissionariesLeft > 0) return false;
            if (state.CannibalsRight > state.MissionariesRight && state.MissionariesRight > 0) return false;
            return true;
        }

        /// <summary>
        /// Selects the node according to the stategy.
        /// </summary>
        /// <param name="fringe">The fringe.</param>
        /// <returns>SearchNode.</returns>
        private static SearchNode SelectNode(Queue<SearchNode> fringe)
        {
            return fringe.Dequeue();
        }
    }
}
