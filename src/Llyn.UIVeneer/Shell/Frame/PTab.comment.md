# PTab.cs

## `public sealed class PTab : Button`

One navigation button carrying its mark and the knowledge of whether its panel is the one shown.
The template keeps the same mark while the chosen state adds its accent surface and bar.

## `public ImageSource? PTabIcon`

The colored mark shown in both the resting and chosen states.

## `public bool PTabChosen`

Whether the tab's panel is the one shown, set by `LNavigation` as the user moves between panels.
