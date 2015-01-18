# 8-Puzzle Solver

This project attempts to solve N-Puzzles by using informed searches. Different weight measures and heuristics (i.a. misplaced tiles, manhattan distance and squared-euclidean distance) are implemented; search algorithms consist of **best-first** search, **greedy best-first** search and **A*** search.

A trivial `M` times `N` puzzle generator is implemented that uses a solvability check for puzzle rejection.

No tree pruning is used, candidate selection however uses a Las Vegas algorithm in order to assist search.