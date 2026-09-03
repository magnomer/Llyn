# PDisplay.xaml

## `<UserControl.Resources>`

The read-only card shapes.
A Meaning card and a Collocation card are one card (LCardDraft).
So the two templates below differ in one element, the Collocation's Expression.
Every member the loaded draft carries is drawn.
A field the card left empty collapses rather than leaving a blank line.

## `<StackPanel x:Name="PDisplayIncomingSection">`

What other entries point at this one.
A card lists its own links, so the entry it points at would otherwise say nothing about them.
Each row opens the entry that carries it, because that is where such a link is edited.
A muted line stands in when nothing translates the entry.

## `<Grid>`

The entry as it reads, with nothing to type into.
The panel that hosts this decides what is selected and when the editor takes its place.
So the same view serves the list panel, the sound panel and the tag panel unchanged.
