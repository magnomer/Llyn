# Favorite

The favorites panel and its roster rows.
It is the one panel browsing marked entries, and its roster leaves as entries are unmarked.

## `QFavorite`

Drives the favorites panel, meaning what it is made of, when it starts and stops, and how it is browsed.
Browsing is the search, the ordering, the roster and the switch between reading and editing.

### `QFavorite`

The rail's print and export buttons sit inside the page, so their commands reach the bindings added here.
The rail has the bin, no new-record button and the export button.
The order menu hangs under the whole search bar.

### `QFavoriteIntroduce`

Only the medium knows its dispatcher, so the marshal the area runs its notices through is built here.
The display view builds the lectern over the editor's display, which Conduct attached to the panel.
The ordering menu is built once, from the orderings `CFavorite` offers.
Nothing is read yet, since `QFavoriteVistaRefine` fills the panel when the workspace opens.
Every change after that arrives as an announcement.
So the panel is current whether or not its tab is in front.
The rail is introduced last, with the atelier's navigation for its trail and the editor for its chronicle.
Every mark and unmark is announced by the engine, wherever the star was clicked.
That is how a row leaves the roster the moment the entry it stands on is unmarked.

### `QFavoriteExitRefine`

The editor and the display stop through `CFavorite`'s closure, which the atelier's exit gate runs.

### `QFavoritePressCheck`

The print button is live when an entry is chosen and the panel is not editing.
An editor on screen prints nothing, because what is printed is what is read.

### `QFavoritePressObserve`

The panel names nothing, and the engine builds the page from stored rows.
Nothing is read back from the screen.

### `QFavoritePortraitObserve`

The gate asks for the file and the format through the envoy.
The engine writes the document from stored rows.

### `QFavoriteModeUpdate`

No control's visibility stands in for the mode.
The rail folds its own button pairs from the scribe flag it is handed.

### `QFavoriteWorkspaceRefine`

The area's workspace event has already closed the chosen entry.
The flags of the workspace that moved are loaded, so the rows built next carry the right ones.
The envoy goes with the load, so the catalog reports a failed load and answers no languages.

### `QRecallObserve`

Typing selects nothing and marks nothing.

### `QFavoriteVistaRefine`

It runs after `CFavorite` has restored its vista and attached its observers.
The flags are loaded before any row is built.
The filter's language menu is then built from the languages that load answers.
The roster is then listed once, so a row keeps the flag it was built with.
The search text stands in the box already, since the restore carried it into the fresh vista.
Its one request is `CFavoriteRowsLoad`, which runs the flag fill and then answers the rows it paints.

### `QRosterRefine`

Before a vista is handed over, or when the read fails, the roster is empty.
A failed read is shown to the user by `CFavorite`.
Ordering by the mark reads when the mark was made, never when the entry was written.
The empty line is shown only while no row stands.
The paint of ready rows asks Conduct nothing.
The parameterless form reads them, and a flag-fill Refine hands in what its load answered.

### `QRosterObserve`

The panel's row gate asks about unsaved changes and records the station.
The click hands over the row's item, so no control decides the request.

### `QRosterApply`

The row carries the `Chosen` cue on the chosen item and none otherwise, which the look sheet paints.
The click is removed first, so a refill never doubles it.
It runs again on every change the item raises, so a chosen row moves without a refill.

## `QRosterItem`

One row of the favorite catalog, with the headword, its language and the flag the language pack carries.
The id is held so the click can name the entry the row stands on.
The engine's row answers every value ready, so the constructor takes them all and defaults none.
A language with no readable flag leaves the image empty and shows none.

### `QRosterItemChosen`

It is the only value of the row that changes after the row is built.
`QSplice` moves the mark in place, so the list keeps its scroll position.

### `QRosterItemName`

The engine numbers repeated headwords as `(1)`, `(2)` on the row it returns.
A repeat is only visible across rows.
`QRosterItemHeadword` keeps the plain headword for everything that is not display.

### `QRosterItemEpithet`

The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.

### `QRosterItemMatch`

The chosen mark is left aside.
`QSplice` keeps the held rows when every pair matches, so their containers survive a refresh.

## Stamp

Commit: `2f9a06b8eb95`
- `QFavorite.cs` `a697381d4473e6af`
- `QRosterItem.cs` `ccf3353eb6ac254d`
