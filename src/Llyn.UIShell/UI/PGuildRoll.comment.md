# PGuildRoll.cs

## `public partial class PGuild`

The roll half of the authors panel: the catalog of Authors, its ordering, its search, and what the engine announces.

## `private void PGuildBulletinHandle(LBulletin bulletin)`

Answers whatever the engine announces by reading the roll again, because every subject can change a count.
A draft bulletin is ignored, because no draft is held here.
A workspace bulletin resets the panel, because every row it showed belonged to the old workspace.
An Author still listed is read again on whichever side is open, and a shown Source is read again too.

## `private void PMusterHandle(object sender, TextChangedEventArgs e)`

Reads the roll again under the typed text.

## `private void PEchelonHandle(object sender, RoutedEventArgs e)`

Takes the chosen ordering from the radio row, stores it, closes the menu and reads the roll again.

## `internal void PEchelonRestore(LCatalogOrder order)`

Reapplies the ordering the last session ended on and reads the roll for the first time.

## `private LCatalogAuthor? PRollCatalogFind(long id)`

The catalog row of one Author, or null while the roll does not list it.

## `private void PRollSelect(long? id)`

Marks one row chosen and every other row not.

## `private void PRollFind(string query)`

Reads every Author matching the text under the chosen ordering, and the Sources crediting nobody with them.
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
