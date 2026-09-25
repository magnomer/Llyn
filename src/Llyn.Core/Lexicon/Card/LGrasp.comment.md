# LGrasp.cs

## `public static class LGrasp`

The user's own judgment of how well they know an entry.
It is a half-step score from zero to five stars, stored as an integer count of half steps.
Zero means the user has not rated the entry yet.
Nothing fetches it and no language pack declares it, so it belongs to the user alone.

## `public const int LGraspStep = 10;`

The highest stored value, five stars counted in half steps.

## `public static bool LGraspCheck(int grasp)`

Whether a value lies within the stored range of zero to ten.

## `public static string LGraspKeyRead(int grasp)`

The localization key of the label for a half-step count, `Grasp.Level0` through `Grasp.Level10`.
