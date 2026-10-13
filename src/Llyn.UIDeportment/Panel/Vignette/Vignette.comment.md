# Vignette

The Repertoire panel's drivers for one Situation: its reading page, its edit area and its entry column.
Each driver subscribes what it paints itself.
So the owner `QRepertoire` only builds, introduces and lights them.
They stand apart from the Corpus panel, whose reading side lives in `QExcerpt`.

## `QVignette`

The reading side, the page one chosen Situation is shown on.
Title at the head, then kind and reference count, then description and media.
Each media row becomes the driver row the editor builds, so no shape crosses into the surface.
The owner's mode refine shows the page while the diptych shows its parent.
Otherwise it shows the body or the unselected notice, as the repertoire says.
The atlas clear empties the media lists, so no video plays on after its Situation is gone.
The tally chip counts the Situation the atlas has chosen.
The Conduct words its sentence through the engine, so a reference added elsewhere shows at the next row notice.
`QScenario` writes the same figure on the writing side, so reader and writer see one figure.
A never-written title arrives muted and paints in the muted colour, because the page head cannot stand empty.
A never-written kind hides its chip.
A description worded by a key shows the key's text as one plain paragraph.
The entry note is drawn the same way, and one painter keeps the two alike.
The description's Markdown resolves its links through the atelier.
Each media list hides itself through the look sheet while it holds nothing.
Null media empties both lists, which also stops any video still playing.

## `QScenario`

The edit area, with three fields, its media rows and its tally chip.
Typing hands each field's text to its own gate on the playwright, which defers it on the tenure.
The gate answers the typed line, and the placeholder and twins are painted from it.
The engine answers with a bulletin, and the fields are redrawn from the draft.
The desk on the playwright holds the tenure from start to commit or cancel.
So the area keeps no draft id or timer.
A text box already reading the same text is left alone, caret included.
The area is live only while `CRepertoireScenarioEnabled` holds.
A Situation has no card, so each media add names a null card and the draft routes it.
The playwright shows a failed hold through the envoy.
The picture and video drivers redraw their own rows.

### Filling without typing

Fields are unsubscribed while filled from a held Situation.
Filling raises the same change as typing, and only typing may reach the gate.
So the area keeps no loading flag.
A closed draft arrives as the blank scenario, so the fields empty without a default here.

### Unseen twins

The title has a hint twin and a ghost twin, which replace what bindings did before.
The hint copies the placeholder and shows only while the line is vacant, so the head row closes on it.
The ghost copies the written title, so the head row closes on the text.
The kind has a measuring twin holding the written kind.
While the kind is empty, the twin takes the line's wording instead.
Each field's placeholder follows the key its line carries.

## `QOccurrence`

The middle column, with the entry search field over it.
It lists the entries referencing the chosen Situation, or every Entry while none is chosen.
The engine matches the typed text and drops hidden languages, so the driver decides nothing about matching.
A failed read is reported by the Conduct occurrence list, which answers no rows, so the driver only fills.
An empty result is shown and not hidden.
Its text says whether nothing references the Situation or nothing matches, picked from whether the vista holds a query.
Rows sharing a headword arrive numbered.
The shown Entry is re-marked after every fill, so its row keeps the mark across a re-filter.
The atelier is taken only to hear the workspace opening, which gives the column its first paint.
That paint waits for the flag load and lists the column once.
It shows the same flags as the catalog.
Its one request is `COccurrenceRowsLoad`, which runs the flag fill and then answers the rows.
A clicked row asks the Conduct to show that Entry.
The Conduct asks about unsaved work before it moves, and keeps the side the panel stood on.
A row paints from its item and repaints on every change the item raises.
So a chosen row moves without a refill.
The click is subscribed once per row, removed first so a refill never doubles it.
The epithet leads with an en space, as its string format did.

## Inline notes

### QOccurrence.cs: Occurrence vista

The Conduct's child vista, whose chosen entry the right-hand side stands on while it shows an Entry.
The right-hand side shows one or the other and never both.
An Entry row swaps the Situation reading for the entry display in place, and a Situation row swaps it back.
The chosen Situation keeps its mark meanwhile, because it still narrows the middle column.

## Stamp

Commit: `c2f335c331fe`
- `QOccurrence.cs` `c87543bf86cbbb8d`
- `QScenario.cs` `a32cda92d7792cce`
- `QVignette.cs` `67b7f75f6b008fe0`
