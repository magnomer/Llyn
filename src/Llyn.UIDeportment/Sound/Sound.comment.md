# Sound

Row items and labels shared by the sound editors and the sound reading views.
Each item is a plain record the editor and the reading view bind alike.
So one entry reads the same in both modes.
They stand apart from the subfolders, which hold whole panels, because these serve several of them.
Values arrive ready from Conduct, so no item holds a session or asks anything.
A property change only tells the view that a value changed and never carries a request.

## `QAccentItem`

One pronunciation row after the primary.
It carries the draft row's id, its variety, the reading text and its audio file.
The text arrives in the form the respelling switch picks, and the brackets follow that choice.
A flip of the switch rebuilds every row, so the fixed brackets never go stale.
The class stays public, since the reading cell binds its text by name.

### Variety

The variety shows as a flag when the pack draws varieties as flags, and as a label otherwise.
The label is blank while a flag stands for it, which collapses the label, so a row never shows both.
The stored name is never the localized label, and `QAccent` matches a spoken accent back to its row by it.
A label is the text under the key Conduct chose, or the raw name when no text exists.
The primary reading, the recorder and the transcription menu label a variety this way too.
The flag comes from `QEnsignImage` under the key Conduct handed, only in flag mode.
The store may not hold it yet, so the flag may arrive after the row and then notifies.
Looking it up again also announces the label, since the label yields to the flag.
The flag key is kept, so a later lookup needs no language.

### Editing

Writing the text only raises the typed event and changes nothing on the row.
The panel hands it to the gate and writes the answer back.
A reading-view row has no listener, so nothing happens there.
Whether the text is the row's respelling is fixed at build, so a typed change becomes a respelling request.
Audio is blank while the row has none, which collapses the play button.
Only the draft's accent writes the audio.
A state refine announces only a real change and never raises the typed event.
So a draft render is never taken for an edit.
Text compares ordinally and a flag image by reference, since WPF seals image equality to reference.

### Row fill

The fill serves `Theme.Accent.Row` and `Theme.Accent.Display`.
The editor row's text shows the placeholder in the muted colour while the row is blank.
It gives the editor reading cell its `QQuillCell` order and tags its slot with `Theme.Accent.Slot`.
Those order strings live here, so the Veneer markup carries none.
It runs again on every change the row raises, so a typed pronunciation or a new flag shows at once.

## `QFrequencyLabel`

What a frequency chip shows, shared by the reading view and the editor.
Neither surface words or colours the chip itself.
[CFrequency](../../Llyn.Conduct/Sound/CFrequency.comment.md) carries the rank, its key and the spare stars.
A null frequency collapses the section, so both surfaces hide it the same way.
The name looks up the key Conduct chose, so the chip speaks the user's language while the engine keeps English.
The rung's brush is the theme resource the tier switches to, and an unknown tier takes the muted brush.
Earned stars take the rung's brush and spare ones take the faint empty brush.
The engine counts the spare stars, so the row stays as wide as the ladder.
The star is a four-pointed glyph, so the row cannot be mistaken for the five-pointed grasp stars.
Spare stars are the same glyph dimmed, since a hollow glyph beside a filled one reads as five stars.
An unranked chip hides the star row and shows the name muted.

## `QGlyphItem`

One character chip of the reading view's glyph row.
The language is the one the character's entry lives in.
The linked verdict is the engine's, and it is false for a rune that is not Han.
An unlinked chip is inert, so punctuation is drawn like the editor field draws it but opens nothing.
Inert means no hit test, no hand cursor and no tooltip.

## `QReflexItem`

One reflex row as the reading view and the input panel bind it.
It carries language, kind, text, romanization, meaning, note, region and the main mark of one draft row.
It also knows whether it leads its language group and whether it is folded away under the visible rows.
Lead, fold and anchor presentation are supplied separately by the list.
The panel's cell editors write the editable properties, and the view only reads.
Editable text and label keys read the held reflex.
That is the last one taken, or its copy after a typed answer.

### Editing

Writing any editable property only raises the typed event, with the cell it names and the text.
The row changes when the gate's answer comes back.
It is accepted as the gate gave it and not as an invented copy.
A null answer names a row the draft lacks, so the row keeps what it holds.
A typed language or kind keys its label from the copy, so the label and tag name the typed text.
The lead is left alone, since the answer's heads reach it through the list.
The binding reads the cell back after the write, so the editor shows the answer.
A reading-view row has no listener.
A label lookup falls back to the raw name on null.
A success is used even when the name is blank.

### Presentation

The kind is printed before the reading as stored, empty when the language sorts by no kind.
The note is printed after the meaning, such as `literary`.
The opener and closer are the ones Conduct chose for the row's language.
The main mark draws the row in the accent colour.
Only a lead row prints the language.
Writing the head field sends the language.
So typing into a blank field starts a new group.
The region is where the reading was taken from, as fetched, never typed.
A blank region arrives as null and binds as the tooltip as it stands.
A multi-character headword and a character without stored placements show no anchor.
State refine takes every value except the anchors in place, so a changed mark or fold needs no new row.
Bracket changes are notified before text changes.
A changed language announces head and label, and a changed kind announces kind and tag.
The folded flag itself raises no property notification.

### Row fill

The fill serves `Theme.Reflex.Row` and `Theme.Reflex.Display`.
A row hidden under the fold is unseen and flat, yet still measured for the shared columns.
A row in common use draws its reading and slashes in the accent colour.
The editor reading cell names the accent field style for such a row, so its box opens accented.
Each side cell gets its `QQuillCell` order and the slot is tagged with `Theme.Reflex.Control`.
Those order strings live here, so the Veneer markup carries none.
The anchor button shows only for an anchorable row.

## `QReflexList`

The reflex rows of one list, shared by the editor's block and the reading view's section.
It owns the rows and the hinge, so both panes reconcile, anchor, lead and fold their rows alike.
It paints the hinge but does not subscribe its clicks.
Every row's typed event is passed on, and the editor hears it while the reading view leaves it unheard.
Showing matches rows by id and refines a kept row in place.
So a row still being typed into keeps its editor.
The hinge shows only when Conduct readies the foldable verdict.
The lead refine pairs each row with the answer `CKindred.CKindredSet` gave for its id.
The editor calls it while a language is typed, before the draft brings the marks back.
Anchor labels are written by id, and a row the labels do not name shows no anchor.
The fold hides each row its reflex's `CReflexHiddenCheck` hides, and shows the opened state on the hinge.
Only the entry page uses the fold, since the series page folds a member's rows as one block.

## `QTranscriptionChoice`

One scheme a transcription row's dropdown offers.
A row keeps one per declared scheme, built once, so the dropdown never rebinds while open.
The taken flag is true while another row of the draft carries the scheme.
The dropdown greys out such a choice, because the engine refuses two rows in one scheme.
It announces only a real change, so a render marking the same state stays silent.

## `QTranscriptionItem`

One transcription row, as the editor and the reading view show it.
The field is bound to the text and the dropdown to the scheme.
It carries one choice per scheme the pack declares.
An empty scheme pick is ignored, because a dropdown rebinding hands one over on its way.
A scheme change announces the label with it.
The owner writes the localization key before the scheme, so the scheme's change announces the label with the new key.
The label shows as a chip before the field, the way a variety name does.
Text announces only a real change, so a render writing the same text stays silent.
A missing translation prints the raw scheme name, and an unnamed scheme prints nothing.
The choices are kept while they match, so an open dropdown is not rebound under the pointer.
They are rebuilt when the count, a scheme name or a label differs from what Conduct answered.
So a renamed or reordered scheme at the same count never leaves a stale label.
Each kept choice takes the taken mark of the scheme at its place.
The shared fill covers scheme name, text and the unseen measure twin.
The editor's own refine in `QTranscription` calls it first, then fills its field and dropdown.
The measure twin is sized by the text, or by the placeholder while the row is blank.
A shown row is brought up to the draft row with the same id.
A changed scheme is written onto the row, which relabels itself.
A row with a text request waiting keeps its text, since the draft is about to become what it holds.

## Stamp

Commit: `c2f335c331fe`
- `QAccentItem.cs` `6182de520abb69ad`
- `QFrequencyLabel.cs` `e5ef5e451796d518`
- `QGlyphItem.cs` `c73eaf6d3021fc9a`
- `QReflexItem.cs` `44f12a638273652f`
- `QReflexList.cs` `fb8f9da6b6cbf7c1`
- `QTranscriptionChoice.cs` `eac30c5878435b53`
- `QTranscriptionItem.cs` `5588437e4b77d737`
