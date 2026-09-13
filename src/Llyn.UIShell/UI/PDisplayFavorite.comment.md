# PDisplayFavorite.cs

## `public partial class PDisplay`

The heart of the reading view: whether the shown entry is marked as a favorite.
The heart shows the stored mark alone and a click writes the mark straight to the workspace.
A mark set from another tab reaches this view through the engine's announcement.

## `private void PDisplayFavoriteShow(long id)`

Reads whether the shown entry is marked and sets the heart to match.
An unknown mark leaves the heart empty rather than claiming the entry is marked.

## `private void PDisplayFavoriteHandle(object sender, RoutedEventArgs e)`

Marks or unmarks the shown entry, following the state the click left on the heart.
A refused write puts the heart back where it stood.
It never shows a mark the workspace does not hold.
Marking creates no entry and changes no lexical data.
