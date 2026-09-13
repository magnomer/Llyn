# TLayout.cs

## `public sealed class TLayout`

Covers the view state the shell stores per tab in the settings file.
That is the ordering each browse panel lists by, the languages it hides, the tab standing open and its split.
Each panel's ordering and filter is pushed on its own and lands in that tab's layout record.
The suite drives one push at a time and reads the whole settings back.
Two pushes in one session must both stand, which is what a whole-record write would break.
A dragged width and a chosen ordering share one record, so neither push may drop the other.
An ordering round-trips by its stored name, and a name this build does not know loads as no ordering.
A missing or unusable mode leaves the window on its opening tab, and only the editor word opens a split.
