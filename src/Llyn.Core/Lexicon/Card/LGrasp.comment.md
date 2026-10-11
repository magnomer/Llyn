# LGrasp.cs
Hash: `b41c99735ab6371c`

## `public static class LGrasp`

The user's own judgment of how well they know an entry.
It is a half-step score from zero to five stars, stored as an integer count of half steps.
Zero means the user has not rated the entry yet.
Nothing fetches it and no language pack declares it, so it belongs to the user alone.

## `public const int LGraspStep = 10;`

The highest stored value, five stars counted in half steps.

## `public static bool LGraspCheck(int grasp)`

Whether a value lies within the stored range of zero to ten.

## `public static int LGraspNormalize(int grasp)`

A value clamped into the stored range of zero to `LGraspStep`.
Both entry writers clamp through it before they read stars or a rating key.
So the Livery and the export sheet never differ on an out-of-range grasp.

## `public static string LGraspKeyRead(int grasp)`

The localization key of the label for a half-step count, `Grasp.Level0` through `Grasp.Level10`.

## `public static IReadOnlyList<int> LGraspStarScan(int grasp)`

The fill of each star for a half-step count, as 0 empty, 1 half or 2 full.
It answers one star per two steps of `LGraspStep`, rounded up, so five stars.
Each star is worth two steps, and a value outside the range clamps per star.
The view's star control and the Livery both read it, so neither keeps its own fill rule.
