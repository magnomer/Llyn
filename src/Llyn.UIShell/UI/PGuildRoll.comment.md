# PGuildRoll.cs

## `public partial class PGuild`

The roll half of the authors panel: the catalog of Authors, its ordering, its search, and what the engine announces.

## `private LVista? _pGuildVista;`

The engine's view state for the guild tab: order, query, and the kinds hidden from the oeuvre.
The panel keeps no copy of any of the three and reads each from the vista where it needs it.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

## `private void PGuildBulletinHandle(LBulletin bulletin)`

Answers whatever the engine announces by reading the roll again, because every subject can change a count.
A vista announcement carrying this panel's vista id reads the roll again, since order, filter or query moved.
The oeuvre is read with it, so a moved filter reaches it the same way.
Another panel's vista is not this panel's business and is skipped.
A draft bulletin is ignored, because no draft is held here.
A workspace bulletin resets the panel, because every row it showed belonged to the old workspace.
An Author still listed is read again on whichever side is open, and a shown Source is read again too.

## `private void PMusterHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement reads the roll again.

## `private void PEchelonHandle(object sender, RoutedEventArgs e)`

Takes the chosen ordering from the radio row, closes the menu and hands the ordering to the vista.
The vista saves it and announces it, and the announcement reads the roll again.

## `internal void PGuildVistaRestore(LVista vista)`

Takes the vista the window started for this tab and puts the panel on it.
The dropdown mark and the filter mark are drawn from it first.
The kind menu is built from its filter.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The roll is then read from the vista.

## `private void PEchelonRestore()`

Moves the dropdown mark onto the ordering the vista holds.

## `private LCatalogAuthor? PRollCatalogFind(long id)`

The catalog row of one Author, or null while the roll does not list it.

## `private void PRollSelect(long? id)`

Marks one row chosen and every other row not.

## `private void PRollFind()`

Reads every Author the vista lists, already matched and ordered, and the Sources crediting nobody with them.
Before a vista is handed over nothing is asked.
The uncredited row stands first while no text is typed, summing the citations of the Sources it stands for.
A chosen Author no longer listed clears the panel, because the row it stood on is gone.
The oeuvre is read again afterwards, because its rows count against the roll's.

## `private void PRollHandle(object sender, RoutedEventArgs e)`

Takes the click of a roll row and shows that Author, after asking about an unsaved name.

## `internal void PRollAuthorShow(long id)`

Chooses one Author: marks its row, drops any shown Source, reads its oeuvre, and reads it on the open side.
The uncredited row chooses nobody, so it reads nothing and offers neither editing nor deleting.

## `internal string PGuildWorkRead(int count)`

The source count as the chips read it, a sentence rather than a bare number.

## `internal string PGuildTallyRead(int count)`

The citation count as the chips read it, in the sources panel's own words.
