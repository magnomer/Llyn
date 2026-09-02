# PDisplay.xaml

## `<UserControl.Resources>`

The read-only card shapes.
A Meaning card and a Collocation card are one card (LCardDraft).
So the two templates below differ in one element, the Collocation's Expression.
Every member the loaded draft carries is drawn.
A field the card left empty collapses rather than leaving a blank line.

## `<Grid>`

The entry as it reads, with nothing to type into.
The panel that hosts this decides what is selected and when the editor takes its place.
So the same view serves the list panel, the sound panel and the tag panel unchanged.
