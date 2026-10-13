# Etymology

The etymology field, drawn by one control so the reading view and the editor show the same thing.
It stands apart because an entry declares its origin in source chips or in a narrative.
The commands it raises are bound by whichever side of the shell draws it.

## `PEtymologyCommand`

Holds the routed commands the field raises: link, unlink, addition, removal and entry.
Link ties the selected narrative words to an entry.
Unlink drops the span under the caret.
Addition takes the typed word as a new source link.
Removal drops the source link the chip names.
Entry opens the entry the chip names.
Its parameter is the chip's entry id, so a handler names no veneer type.

## `PEtymon`

One item of the field's collection, so the chips and the typing entry wrap as one line.
The caret item ends the chips, and the typed word waits there before it becomes a link.
Each field builds one caret item and keeps it last across every redraw of its chips.
So a word half typed survives a chip being added.
The word lives on the item and not on the box, which is rebuilt whenever a chip is added.
Its two values are dependency properties, so the drawn box follows them without a hand-raised change event.
The chip constructor takes an entry id, a headword and a language, and draws the language's flag.
The drivers build one chip per resolved source link and hand the list to the field.
`PEtymonText` is bound two ways to the box, and clearing it after a pick clears the box.
`PEtymonShown` is true only on an editable field.
The read-only field keeps the item but collapses its box, so no branch picks the collection.

## `QEtymology`

Holds the source chips and the narrative together.
Editable, it adds an entry the user types a source into and a box for the narrative.
Read, it prints the narrative with its spans underlined and the chips beside it.
The host decides whether the whole field shows, since the editor's tab and the reading view differ.
Every visibility verdict comes ready from the driver that owns the instance, so the control only draws.
`QEtymologyEditable` says which side draws it, set by the editor that owns the editable instance.
`QEtymologyNarrated` is the ready verdict for the narrative's read face, false unless the reading view hands it.
`QEtymologyCard` says the frame is the editor card with a head strip, set by the editor when it attaches.
The constructor builds the chips row, both narrative faces, the frame and the head strip once.
The templates come from the theme, so the two sides cannot drift apart.
`QEtymologyBox` is the narrative box, which the editor reads a selection from.
`QEtymologyLine` is the chip line under that box, where the editor paints the ready chips.
`QEtymologySourceShow` draws the items a driver built from links the engine already resolved.
The field holds no Conduct record, so the display driver and the editor each hand it items.
The typing entry always closes the row, collapsed on the read side, so the row has one shape.
`QEtymologyItemApply` fills one item of `Theme.Etymology.Item`.
The removal button shows only while the field is editable.
The entry takes the typed text both ways and the addition command on Return.
It watches the item's text and shown properties, since an etymon raises no property change.
`QEtymologyItemRefine` fills the items again when either changes.
`QEtymologyBoxRefine` writes the typed text back to the caret etymon.
`OnCreateAutomationPeer` gives a surface peer, since the control is a box and not a button.

## Inline notes

### QEtymology.cs: `QEtymologyStateRefine`

Every property redraws the whole field, which is small enough to build again.

### QEtymology.cs: `QEtymologyStateApply`

Paints which narrative face stands, from the mode and the verdict the owning driver set.
It also picks the frame look and shows the head strip from the card flag.
The prose takes the narrative as one plain piece through `QMentionPieceCreate`, since mention text takes only pieces.

## Stamp

Commit: `c2f335c331fe`
- `PEtymologyCommand.cs` `14fe494d4bf70f5c`
- `PEtymon.cs` `22509bed0253c539`
- `QEtymology.cs` `13e023d1cd7d49bc`
