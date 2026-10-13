# Tenor

This folder holds the tenor panel's session and the entry list of the chosen Register.

## `CCohort`

The tenor's entry list, holding the entries that carry the chosen Register and the one the reader stands on.
It is shaped like `CQuotation`, and the tenor owns it as `CCorpus` owns its quotation.
Its panel reports a failed load as `Register.LoadFailed`.
Its empty list reads `Register.Unmatched` under a search and `Register.Vacant` without one.
The panel asks the editor's desk before it leaves an entry, and it finishes through the editor.
It takes the fresh register vista as its roll and hands the fresh cohort vista to the panel's aperture.
An entry notice goes to the panel, whose chosen row it may name.
A vista notice raises the entry rows alone, since only the entry search moved.
A notice on the chosen entry rereads its draft.
The rows are the entries under the chosen Register, as the engine narrows them.
Print and export do nothing while no entry is chosen or the panel is editing.
The print window chooses the printer and layout, and a window closed unprinted prints nothing.

## `CTenor`

The tenor panel owns the Register list, the entry editor and the entry list it hands to `CCohort`.
It mirrors `CTaxonomy` member for member, with Registers for Tags.
Its keys are `Register.LoadFailed`, `Register.CreateFailed` and `Coinage.Register`.
Its gates are `CTenorRegisterToggle`, `LTenorRegisterOpen`, `LTenorCoinageAllowed`, `CTenorRowsRead` and `CTenorEntryCreate`.
`CTenorRegisterOpened` and `CTenorWorkspaceChanged` follow the same timing as the taxonomy's events.
A Register notice carries a Register id, not an entry id, so it never selects a row.

## Stamp

Commit: `c2f335c331fe`
- `CCohort.cs` `1840837f1e9f29bf`
- `CTenor.cs` `3aca57e497ef871a`
