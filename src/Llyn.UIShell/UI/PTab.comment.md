# PTab.cs

## `public sealed class PTab : Button`

One navigation button carrying two marks and the knowledge of whether its panel is the one shown.
The template shows the outline mark at rest and the filled mark with an accent bar while chosen.
Holding both geometries on the control lets one style serve every tab without a second style to swap in.

## `public Geometry? PTabOutline`

The mark drawn in the tab's own foreground while its panel is hidden.

## `public Geometry? PTabFilled`

The mark drawn in the accent while its panel is shown.

## `public bool PTabChosen`

Whether the tab's panel is the one shown, set by the window as the user moves between panels.
