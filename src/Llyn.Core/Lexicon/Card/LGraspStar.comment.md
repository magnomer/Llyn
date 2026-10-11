# LGraspStar.cs
Hash: `9cd58857f207f8db`

## `public sealed record LGraspStar(IReadOnlyList<IReadOnlyList<int>> LGraspStarFill, IReadOnlyList<bool> LGraspStarUnrated, IReadOnlyList<IReadOnlyList<int>> LGraspStarHit)`

Everything a star control draws and hit-tests for one limit, answered before any pointer moves.
The view only looks answers up by step or star, so it computes no grasp rule.

**Parameters**

- `LGraspStarFill`: per step from zero to the limit, the fill of each drawn star.
  Each fill is 0 empty, 1 half or 2 full, as `LGrasp.LGraspStarScan` answers it.
- `LGraspStarUnrated`: per step, whether the stars show the unrated look, true only at step zero.
- `LGraspStarHit`: per drawn star, the step its left half picks and the step its right half picks.

## `public static LGraspStar LGraspStarResolve(int limit)`

Builds the answers for a control whose last step is `limit`, a negative limit counting as zero.
It draws one star per two steps of the limit, rounded up, never more than Core's five.
So a limit of zero draws no star and answers no hit.
A half never picks a step above the limit, so an odd limit's last star tops out at its half.
The fills come from `LGrasp.LGraspStarScan`, the only copy of the fill rule.
