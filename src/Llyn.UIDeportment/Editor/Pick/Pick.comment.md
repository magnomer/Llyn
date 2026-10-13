# Pick

Pick holds the shared dropdowns of stored rows that a typed wording may already name.
Each is one popup the editor owns rather than one per card.
Only one field is typed into at a time.
The popup is retargeted at the caret that opened it.
A card built at runtime could not declare a popup of its own.

## `PProfferItem`

This is one stored row offered in the dropdown, with the wording shown and the id it is stored under.
The id is what the pick carries.
So choosing a row links the stored row rather than a copy of its words.
Every find builds its rows through the constructor, which copies a ready row whose split and count the engine made.
A register row carries an empty count, so it shows none.
The wording comes in three pieces, split by Core's `LCatalog.LCatalogMarkFind` before the row arrives.
The row draws the middle piece in weight, so a reader sees why the row is offered.

## `PProspectItem`

This is one row of the dropdown that opens when typed text matches more than one Entry.
Most rows stand for an existing Entry and carry its id.
The closing rows offer a fresh Entry per language, with a null id, so no magic id stands for one.
Only those rows are marked fresh, read from the null id.
The flag is derived from the language so the dropdown reads like every other headword list.
The shown name defaults to the headword and is numbered `(1)`, `(2)` while another row carries the same one.
`LTwin` writes the number once the list is filled, because a repeat is only visible across rows.
The epithet is empty when the setting is off or the entry keeps none.

## `QProffer`

This driver serves the Situation caret, the Register caret and the citation field of a sentence row alike.
A Situation is a workspace entity with a description, a kind and a usage count.
Two cards naming the same wording should name the same Situation, or the workspace would hold two that read alike.
The list opens as the wording is typed, because a user cannot pick from a catalogue they were never shown.
The dropdown keeps no note of the field it serves, and a pick reads the field that holds focus.
It never takes focus, and it shuts when that field is left.
The Situation and Register drivers hold it in turn, since each hands it their finds.

### `QProfferApply`

The miss Refine is subscribed before the pick Observe, so each hears the press in its role.

### `QProfferPickObserve`

A Register caret and a Situation caret each link their stored row at the caret.
The citation field cites the chosen Source on its sentence row through `CSentenceCitationSet`.
The press is heard before focus moves, so that field is still the one the list was opened for.
The clerk skips a row the card already carries.

### `QProfferSituationRefine`

The gate matches the wording anywhere inside a stored title.
Rows come most used first, and the list stays shut when nothing matches.

### `QProfferCitationRefine`

The field keeps its own text, since typing a citation writes nothing.
The gate offers nothing for the byline the row already cites.

### `QProfferOpenRefine`

Every find's Refine shares it, so the popup looks the same whichever field opened it.

### Constructor

The dropdown hangs under the frame of the caret it was opened from, not under the caret itself.
The frame is the edge a reader sees, and it is drawn outward of the caret.
Where it lands is settled in code, and the gutter the surface keeps for its shadow is taken back exactly.
Rows, corners and shadow are cut down from the translation dropdown's, which is drawn against a far bigger field.
A row carries the wording at the size the chip will carry it.
So what is offered is read as what will be taken.
It carries no line around it.
The shadow already says where it ends, and a line made it a second card.

### Inline notes

### QProffer.cs: `internal void QProfferKeyRefine(object sender, KeyEventArgs e)`

The dropdown never takes focus, so the field's keys drive it.
A shut dropdown holds no rows, so the gate answers null and the arrow stays unhandled.
Each field subscribes it before its own key handlers, so a key it takes ends there.

### QProffer.cs: `private void QProfferCloseRefine(object? sender, EventArgs e)`

A shut dropdown keeps no selected row and no rows.
So Enter reaches a stored row only while the list stands open.

### QProffer.cs: `internal void QProfferKeyObserve(object sender, KeyEventArgs e)`

Nothing is selected while the list merely stands open, so Enter still commits the typed wording.

### QProffer.cs: `QProfferPopup.PlacementTarget = QProfferFrameFind(box) ?? box ?? (UIElement)QProfferContents`

The frame is the edge the dropdown is read against, with the caret and then the pane as fallbacks.
The list still opens when the frame is not built yet.

### QProffer.cs: `private static CustomPopupPlacement[] QProfferPlace(Size popup, Size target, Point offset)`

The dropdown is placed by hand from the frame's own corner.
Placing it under the edge left it over the caret.
The second placement puts it above the frame, for a caret near the foot of the screen.

## `QProspect`

This driver offers the Entries a typed translation may link to.
It sits beside the Proffer popup the editor also owns.
The pronunciation lookup and the part-of-speech menu share the shape.
A typed translation opens it at a card's link caret, offering matches and then a create row per language.
A selected or typed word opens it at the selection, offering matches only, for a mention or an etymon.
The corpus scribe reaches the second through its own editor, so the popup is still declared once.
Its shutting drops the selection, so Enter never takes a row the dropdown no longer shows.

### `QProspectPlaceRefine`

An opener places the picker first, then paints the offer its Conduct read or gate answered.

### `QProspectOpenRefine`

The blank word, the find, the self filter, the languages and the failure notice are all settled below.
The stored rows come first, then one create row per offered language.
A mention's offer carries no language, because a Mention may only point at an Entry that already exists.
The create row carries the typed word untouched, because that word is what the tentative entry will be called.
A picked row is inserted through the caret gate, which places it at the trail's own position.

### `QProspectTranslationRefine`

A dropdown opened while the user types selects nothing, so Enter still means what was typed.
One opened because a committed word was ambiguous selects its first row, since the user must choose.

### `QProspectPickObserve`

The row is a plain surface rather than a button, so the list beneath it keeps its own selection.
Every pick hands the raw row, a nullable id with the word and language it carries.
A create row has a null id, and the gate alone decides what that means.
The translation gate starts a tentative entry for it first, then links it.
The driver never tests the id, so no value of it skips a gate or picks another.
Any field other than a link caret, etymon entry or sentence runs the mention pick command on itself.
The field's text and selection are read at the pick, raw, since the dropdown never takes focus.

### `QProspectKeyRefine`

Escape and the arrows only change the open dropdown and ask no gate.
A key it does not use falls through to the entry's later handlers.

### `QProspectCloseRefine`

A click elsewhere shuts the dropdown without `QProspectShutRefine`.
A kept selection would let Enter take a row no one sees.

### `QProspectKeyObserve`

It runs after the dropdown's Refine and before the entry's own Enter, so a chosen row wins.

### `QProspectShutRefine`

It also takes back any offset a mention placing set.

### `QProspectApply`

The plus of a create row and the epithet's leading space are set here.
A data trigger and a string format stood there before.

## Stamp

Commit: `c2f335c331fe`
- `PProfferItem.cs` `2c61e99019035e61`
- `PProspectItem.cs` `0db2c8c692a1358d`
- `QProffer.cs` `b6854fd93d6f1f9a`
- `QProspect.cs` `6aea75238707bd16`
