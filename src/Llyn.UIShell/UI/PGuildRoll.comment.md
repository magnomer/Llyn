# PGuildRoll.cs

## `public partial class PGuild`

The roll half of the authors panel: the catalog of Authors, its ordering, its search, and what the engine announces.

## `private LVista? _pGuildVista;`

The engine's view state for the guild tab: order, query, the chosen Author, and the kinds hidden from the oeuvre.
The panel keeps no copy of any of the four and reads each from the vista where it needs it.
The uncredited row is chosen as zero, the id no stored Author carries.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

## `private void PGuildCatalogUpdate()`

A stored Author, Source, Example or Entry can change a name or a count, so the roll is read again.
An Author still listed is read again on whichever side is open, and a shown Source is read again too.

## `private void PMusterHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement reads the roll again.

## `private void PEchelonHandle(object sender, RoutedEventArgs e)`

Takes the chosen ordering from the radio row, closes the menu and hands the ordering to the vista.
The vista saves it and announces it, and the announcement reads the roll again.

## `internal void PGuildVistaRestore(LVista vista, LVista oeuvre)`

Takes the two vistas the window started for this tab and puts the panel on them.
The roll vista holds the roll's order and query, the chosen Author, and the kind filter.
The oeuvre vista holds the oeuvre's query and the chosen Source.
Each subject the panel cares about is attached once, so no handler sorts announcements by subject.
A roll vista announcement reads the roll again, since order, filter or query moved.
The oeuvre is read with it, so a moved filter reaches it the same way.
An oeuvre vista announcement reads the oeuvre alone, since only its query moved.
Each vista forwards its own announcement only, so neither is mistaken for the other.
A workspace bulletin resets the panel, because every row it showed belonged to the old workspace.
A draft bulletin is not attached, because no draft is held here.
The dropdown mark and the filter mark are drawn from the roll vista first.
The kind menu is built from its filter.
Search text still standing in either box is handed to its vista, so a switched workspace keeps both searches.
The roll is then read from the vistas.

## `private void PEchelonRestore()`

Moves the dropdown mark onto the ordering the vista holds.

## `private LCatalogAuthor? PRollCatalogFind(long id)`

The catalog row of one Author, or null while the roll does not list it.

## `private void PRollChosenApply()`

Marks the row of the Author the vista stands on and clears the mark from every other row.
No row is marked when the vista stands on none, which is what a cleared panel shows.

## `private void PRollFind()`

Reads every Author the vista lists, already matched and ordered, and the Sources crediting nobody with them.
Before a vista is handed over nothing is asked.
The uncredited row stands first while no text is typed, summing the citations of the Sources it stands for.
A chosen Author no longer listed clears the panel, because the row it stood on is gone.
The oeuvre is read again afterwards, because its rows count against the roll's.

## `private void PRollHandle(object sender, RoutedEventArgs e)`

Takes the click of a roll row and shows that Author, after asking about an unsaved name.

## `internal void PRollAuthorShow(long id)`

Chooses one Author into the vista and marks its row.
Any shown Source is dropped, the oeuvre read, and the Author read on the open side.
The uncredited row chooses nobody, so it reads nothing and offers neither editing nor deleting.

## `internal string PGuildWorkRead(int count)`

The source count as the chips read it, a sentence rather than a bare number.

## `internal string PGuildTallyRead(int count)`

The citation count as the chips read it, in the sources panel's own words.
