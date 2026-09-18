# PGuildOeuvre.cs

## `public partial class PGuild`

The middle half of the authors panel: the Sources crediting the chosen Author, their narrowing, and the Source read.

## `private LVista? _pOeuvreVista;`

The engine's view state for the oeuvre list: its query and the chosen Source.
It is null until the window hands one over, so the handlers do nothing before that.

## `private void PCombHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the oeuvre vista, whose announcement reads the oeuvre again.

## `private void PLouverHandle(object sender, RoutedEventArgs e)`

Takes the ticked kinds from the menu and hands them to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

## `private void PLouverRestore()`

Shows the filter mark while the vista hides any kind.

## `private void PLouverBuild(LCatalogFilter filter)`

Draws one tick row per source kind, worded by the sources panel's own key and tagged by the stored word.
The rows are drawn in code because a kind is not a language, and the shared filter builder draws flags.

## `private LCatalogReference? POeuvreCatalogFind(long id)`

The catalog row of one Source, or null while the oeuvre does not list it.

## `private void POeuvreChosenApply()`

Marks the row of the Source the oeuvre vista stands on and clears the mark from every other row.
No row is marked when the vista stands on none, which is what a cleared panel shows.

## `private void POeuvreFind()`

Reads the Sources crediting the chosen Author from the two vistas, narrowed by the typed text and the hidden kinds.
Before both vistas are handed over nothing is asked.
No Author chosen reads every Source, and the uncredited row reads the Sources crediting nobody.
The empty text says which emptiness it is: no Sources at all, none for this Author, or none matching.
A shown Source no longer listed is dropped, because the row it stood on is gone.

## `private void POeuvreHandle(object sender, RoutedEventArgs e)`

Takes the click of an oeuvre row and shows that Source, after asking about an unsaved name.

## `private void PColophonReferenceShow(long id)`

Chooses one Source into the oeuvre vista and reads it into the colophon, in place of the Author reading.
The credits and the citation count come from the catalog row, because the oeuvre already holds them.
Editing ends first, because the surface shows one thing at a time and a Source is not edited here.
The mode toggle and the bin go dead, because neither applies to a Source.

## `private void PColophonReferenceHide()`

Drops the shown Source from the oeuvre vista and brings the Author reading back.
The toggle and the bin go live while an Author stands.
