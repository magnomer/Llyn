# Draft

This folder holds the records the editor's parts read when the desk announces a held draft.
They mirror only what the controls show, so the full engine draft stays between controllers.
A stored row has a positive id, and a fresh one carries zero or a nonpositive id.

## `CDraft`

The held draft as a driver reads it when the desk announces it.
It carries only what a driver writes into its controls.
`CDraftAuthorName` is empty for any draft that is not an author.

## `CEntryDraft`

The held entry as the editor shows it whenever the desk announces it.
It mirrors only what the editor's parts read.
It holds the headword, the note, the meaning cards and the collocation cards.
The etymology boxes are not part of it, since the etymology gate reads them from the held draft itself.

## `CMarker`

The part of speech field as the editor shows it, with the lexical unit and freedom droppers heading its row.
The speeches are the committed parts, one chip each by its shown name.
The typed text is the pending text the field keeps.
The category menu serves both the pending text and the chips.
The units and freedoms are offered to every draft, each as a localization key and whether the draft holds it.
They are plain pairs, so the unit menu adds no type to what Conduct offers its drivers.

## `CReflexDraft`

One reflex of an entry, as the reflex rows show it.
Its shown reading comes from the row's guise, not from this shape.
`CReflexDraftMain` is whether the reflex is the language's main one.
The anchors are the fanqie readings the reflex anchors to.
`CReflexDraftRead` shapes the reflexes of a draft for the reflex rows and the lectern.
The private overload copies the fields the rows read from one engine reflex draft.

## `CStep`

One row of the editor's walk, which both faces step through in the same order.
A row with no card is the headword or the section tabs.
Those are the first two rows of every section.
Only a card row holds a card, and only a stored card holds a fold gate.
The gate answers whether the fold went through, and a card that cannot fold carries none.
A card row also carries its carets, so a face draws them without asking Conduct again.

## `CTranscriptionDraft`

One transcription of an entry, as the transcription and glyph rows show it.
`CTranscriptionDraftKey` is the localization key the scheme of the row is labelled under.
`CTranscriptionDraftRead` shapes the transcriptions of a draft for the transcription rows and the lectern.

## Stamp

Commit: `c05429890cec`
- `CDraft.cs` `b3bb4baa7849e72b`
- `CEntryDraft.cs` `4dcad69c4d192ac1`
- `CMarker.cs` `5593c710aee3c7f3`
- `CReflexDraft.cs` `6d3c5be31f5d104d`
- `CStep.cs` `cb5a353cd1c59d99`
- `CTranscriptionDraft.cs` `b28f91481756e9ef`
