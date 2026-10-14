# Transcription

The editor's transcription rows and the lookup menu shared by the pronunciation, transcription and glyph rows.
It stands apart because the menu serves three row kinds.

## `QTranscription`

The editor's driver for the transcription rows of the input panel, rendered from the draft the engine holds.
They stand beneath the pronunciation rows, one per scheme, and only for a language whose pack declares schemes.
A row shows its scheme as a dropdown, then the field, its lookup button and the plus and minus.
The dropdown offers every declared scheme and greys out one another row holds.
It wears no brackets and no audio, because a transcription is a spelling and not a sound.
A row edit goes to the text gate and a picked scheme to the scheme gate.
The scheme gate is unhooked while the fill sets the selection, so only a user's pick is heard.
A plus goes to the add gate.
A null row is Conduct's word for append, so no magic id is sent.
The engine picks the free scheme and the place after the row.
The plus is enabled from the free verdict the last paint stored.
So it never offers a row the engine would refuse.
A minus goes to the remove gate, and the engine's draft bulletin takes the row off the screen.
Lookup opens the editor's one menu under the row's own button.
The start request carries only the row id, and Conduct reads the row's scheme from the held draft.
The menu opens before the search observe, so the old search closes first.
Rows are rendered as a diff on each draft bulletin, so a row being typed into is left alone.
The row keeps the draft's text until the draft returns with the typed one.
The dropdown shows each choice by its label and selects by its scheme.
The paths are set in code and not in the style.
Each dropdown takes the schemes its ready row carries, inside the pairing of `QLookItem.QLookItemShow`.
That pairing matches rows one to one, so a repeated id never gives one row two scheme lists.
A stored row in a scheme the pack no longer declares is still shown, because it is the entry's data.
The read leaves out the glyph row, which the glyph block shows beneath.

## `QTranscriptionCommand`

The routed commands a transcription row raises, bound by the editor that hosts the rows.
Each carries the row as its parameter, because the row template has no code of its own.
Addition adds a row in the next scheme the draft does not yet hold, after the row.
Removal drops the row.
Notation opens the lookup menu, searching in the row's own scheme.

## `QNotation`

The editor's driver for the pronunciation and transcription lookup menu.
Every pronunciation, transcription and glyph row carries its own lookup button.
The menu is one popup the editor owns, retargeted at the row that asked for it.
Opening starts a search for the headword, the same search from whichever row.
The search is a foray the desk's errand starts and keeps, so the menu holds no handle.
Candidates stream in from the engine, and the errand keeps the rows, one reading per variety a source returned.
Picking one hands it to the errand, which writes it into the row that opened the menu.
The errand marshals each step onto the dispatcher and raises the ready notation state.
So the menu implements no contract and decides nothing about a step.
The primary row's target is null, Conduct's word for no row.
The popup is shut before it is placed, so a press on another row's button moves it.
Shutting it stops the old search before the new start.
A closed menu stops the search, so a stale arrival finds no foray.
A pick hands the reading's phonetic and variety to the errand and closes the menu.
The start paints the state, then loads the flags and paints again.
So readings that landed before the flags gain them, since every paint rebuilds the rows.
The flags load after the start, as the clip popup's do.
Each paint builds fresh rows, so a changed flag or notice always shows.

## `QNotationItem`

One row of the menu, for one source, built from the errand's ready row.
It carries the looked-up label, flag and notice text, and decides nothing about them.
Readings keep their raw phonetic and variety, which a pick hands back.
They are listed in arrival order, empty while none has arrived.
A row without a reading says one line: searching, no entry, or failed to retrieve.
Ready means the row carries a reading the user can take now, as `CClipItemReady` does on an audio row.
The template switches readings and notice on it.

## `QNotationReading`

One transcription a source returned, as one takeable button.
It is built once and never changes, so it carries no change notification.
The raw variety is what the draft stores, and never the label the user sees.
The label is localized when a `Variety.*` key exists, and raw otherwise.
In flag mode the label is the flag's tooltip and not visible text.
The flag is resolved when the pack shows varieties as flags and declares one.
Null means the label stands in.
The phonetic is what taking the reading stores.
The engine derives the respelling from it again, so the pick fills both forms.
The printed text is the respelling while the switch shows respellings, and the phonetic otherwise.
A transcription lookup carries no respelling and prints its text as it came.
The opener and closer are blank for a transcription, slashes for a phonemic respelling and square otherwise.
A flagged reading shows the flag and hides the name.
An unflagged reading shows the name when it has one and offers no tooltip.
The column joins a shared size group named for the reading's alternation index.
So readings at the same place in every source row line up.

## Stamp

Commit: `c6e60141a172`
- `QNotation.cs` `a365d17064fc01b0`
- `QNotationItem.cs` `8a72ca494c1fb157`
- `QNotationReading.cs` `3d289fd0b9fc5b84`
- `QTranscription.cs` `9f2dc2844a058e9f`
- `QTranscriptionCommand.cs` `3d3b1bce6453b585`
