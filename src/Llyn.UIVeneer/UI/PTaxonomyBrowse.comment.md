# PTaxonomyBrowse.cs

## `public partial class PTaxonomy`

Browsing behavior of the taxonomy panel.
The search field and the sorting dropdown refill the tag catalog.
A chosen tag refills the entries beside it, and a chosen entry is loaded back from the workspace.
It is rendered read-only in the reader, which the mode toggle swaps for the editor.
This is the same read half of the entry round trip the library panel offers.
It is reached through a tag instead of a headword.
The entry list itself lives in [PTaxonomyMembership.cs](PTaxonomyMembership.comment.md).

## Inline notes

### `private LTaxonomy _lTaxonomy = null!;`

The panel's deportment, holding the tag vista and the Membership vista the window restored.
The vista carries the order, the query, the chosen Tag, and the languages hidden from its entries.
The panel keeps no copy of any of the four and asks the deportment for each where it needs it.
The chosen tag is null when none is chosen, which is the whole workspace, not an absence to be corrected.
It is null until the window hands one over, so the handlers do nothing before that.
A switched workspace hands over a fresh vista, read from that workspace's own layout.

### `private async void PTaxonomyWorkspaceUpdate()`

A workspace that moved empties the panel and reloads its flags before any row is built.

### `private void PTaxonomyTagUpdate()`

A tag announcement carries a tag id, not an entry id, so it never selects a row.
It rebuilds the catalog and re-reads the shown entry, whose chips may carry the renamed tag.
Passing it on as an entry would make a fresh entry being written adopt the tag's id.

### `private void PExplorationHandle(object sender, TextChangedEventArgs e)`

Each keystroke hands the search text to the vista, whose announcement rebuilds the catalog.

### `private void PFunnelHandle(object sender, RoutedEventArgs e)`

A chosen ordering closes the dropdown and hands the ordering to the vista.
The vista saves it and announces it, and the announcement rebuilds the catalog.

### `private void PLatticeHandle(object sender, RoutedEventArgs e)`

The ticked languages are read off the menu and handed to the vista, which saves and announces them.
The mark on the button is redrawn from the vista at once.

### `internal async void PTaxonomyVistaRestore()`

The deportment starts the tab's vistas from the window's posture, so no vista crosses the veneer.
Takes the vista the window started for this tab and puts the panel on it.
The panel answers the engine through the vistas rather than its own visibility.
So a tag written in the input panel is in the catalog at once, with no tab switch needed.
Each subject the panel cares about is attached once, so no handler sorts announcements by subject.
A vista announcement rebuilds the catalog, since order, filter or query moved.
The entry list is rebuilt with it, so a moved filter reaches it the same way.
A reflex fill or a flipped setting rewrites the epithet beside a headword, so each rebuilds too.
An entry announcement goes to the membership vista, whose chosen row is the entry it may name.
A draft edit or a fetched frequency, paradigm, script or fanqie row changes no listed row, and is not attached.
The dropdown mark and the filter mark are drawn from it first.
The flags are loaded before any row is built, then the language menu is built from the loaded packs.
Search text still standing in the box is handed to the vista, so a switched workspace keeps the search.
The catalog is then listed from the vista.
The membership vista is kept for the entry column and handed to the display, which reads its chosen entry.
The membership vista carries the entry search box, so its announcement refills the entry column alone.

### `private void PLatticeRestore()`

Shows the filter mark while the vista hides any language.

### `private void PDirectoryReset()`

Lets go of the chosen tag, so a switched workspace opens on every entry.

### `private void PTaxonomyFreshHandle(object sender, RoutedEventArgs e)`

New makes whatever the emptier panel would list.
With no tag chosen and no entry shown, it names a new tag.
With a tag chosen, or an entry shown, it starts a new entry.

### `private void PDirectoryTagCreate()`

Asks for the wording, makes the tag, and browses by it.
A dismissed dialog changes nothing.
The panel is cleared first, so an unsaved fresh entry the leave check already settled does not linger.

### `private void PDirectoryFind()`

The engine returns the tags answering the vista, already in its ordering.
The entries under a tag stay in headword order, which the database already gives them.
The chosen tag is re-marked as the catalog is rebuilt, so the selection survives a re-sort.
A chosen tag the rows no longer hold is dropped through the vista, and the list is built unmarked.
The panel then falls back to every entry.
Before a vista is handed over nothing is asked.

### `internal void PDirectoryTagShow(long id)`

Browses by one tag for a caller outside the panel.
That is how a tag chip read on a card reaches this panel.
The query is emptied first.
A tag left out by the standing query would be chosen and dropped in the same breath.
The tag is then chosen through the vista and the catalog rebuilt, since it may be new to the list.

### `private void PDirectorySelect(long? id)`

Chooses one tag, or none, and re-marks the rows in place before listing the entries under it.
No row is rebuilt, so the list keeps its scroll and its focus.

### `PDirectorySelect(item.PDirectoryItemChosen ? null : item.PDirectoryItemId);`

Clicking the chosen tag lets go of it.
That is how the panel is put back on the whole workspace without a separate control saying so.

### `internal void PTaxonomyScribeRestore(bool editing)`

Puts the panel back on the side it was left standing on.
The button is enabled first when the editor is the side restored.
An empty editor is the state a new record is written in.

A session that ended on the editor with nothing selected comes back on the reading side instead.
Otherwise the launch would open a blank draft nobody asked for.

## `internal long PTaxonomyVoyageRead()`

The tag the panel shows, as the station the window records before a jump away.
Zero says no tag is shown, so there is no place to come back to.
