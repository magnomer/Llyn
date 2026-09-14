# PGuildVita.cs

## `public partial class PGuild`

The reading half of the authors panel: one Author on the page, its co-authors, and the places citing it.

## `private LCatalogAuthor? PVitaCatalogRead(long id)`

The catalog row of one Author, from the roll when it lists it and from the whole catalog otherwise.
The whole catalog is asked because a co-author reached from the vita may be hidden by the typed search.

## `internal void PVitaShow(long id)`

Reads one Author onto the page: its name, its two counts, its co-authors and the places citing its sources.
An Author the catalog no longer holds clears the panel.

## `private void PVitaNameShow(string name)`

Writes the name at the head of the page, or the muted unnamed text for a blank name.

## `private void PFellowShow(long id)`

Reads the Sources crediting the Author and counts every other credit on them, one row per co-author.
The rows stand busiest first and then by name, and the heading hides while there are none.

## `private void PVitaCitationShow(long id)`

Reads the places citing the Author's sources and draws one row per card or lone Example.
Each row is worded by its kind, so a meaning, a collocation and an example read apart.
Repeated headwords are numbered as the display numbers them.

## `private void PFellowHandle(object sender, RoutedEventArgs e)`

Takes the click of a co-author row and chooses that Author, after asking about an unsaved name.

## `private void PVitaCitationHandle(object sender, RoutedEventArgs e)`

Takes a citation row and leaves for its Entry in the library or its Example in the corpus.

## `private void PVitaClear()`

Empties the page and shows the prompt in its place.
