# PGuildOeuvre.cs

## `public partial class PGuild`

The middle half of the authors panel: the Sources crediting the chosen Author, their narrowing, and the Source read.

## `private void PCombHandle(object sender, TextChangedEventArgs e)`

Reads the oeuvre again under the typed text.

## `private void PLouverHandle(object sender, RoutedEventArgs e)`

Takes the ticked kinds from the menu, stores them, marks the button while any hides, and reads the oeuvre again.

## `internal void PLouverRestore(LCatalogFilter filter)`

Reapplies the hidden kinds the last session ended on and builds the menu rows.

## `private void PLouverBuild(LCatalogFilter filter)`

Draws one tick row per source kind, worded by the sources panel's own key and tagged by the stored word.
The rows are drawn in code because a kind is not a language, and the shared filter builder draws flags.

## `private LCatalogReference? POeuvreCatalogFind(long id)`

The catalog row of one Source, or null while the oeuvre does not list it.

## `private void POeuvreSelect(long? id)`

Marks one row chosen and every other row not.

## `private void POeuvreFind()`

Reads the Sources crediting the chosen Author, narrowed by the typed text and the hidden kinds.
No Author chosen reads every Source, and the uncredited row reads the Sources crediting nobody.
The empty text says which emptiness it is: no Sources at all, none for this Author, or none matching.
A shown Source no longer listed is dropped, because the row it stood on is gone.

## `private void POeuvreHandle(object sender, RoutedEventArgs e)`

Takes the click of an oeuvre row and shows that Source, after asking about an unsaved name.

## `private void PColophonReferenceShow(long id)`

Reads one Source into the colophon, in place of the Author reading.
The credits and the citation count come from the catalog row, because the oeuvre already holds them.
Editing ends first, because the surface shows one thing at a time and a Source is not edited here.
The mode toggle and the bin go dead, because neither applies to a Source.

## `private void PColophonReferenceHide()`

Drops the shown Source and brings the Author reading back, the toggle and the bin live while an Author stands.
