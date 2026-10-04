# CEntryDraft.cs
Hash: `6517490298000267`

## `public sealed record CEntryDraft(string CEntryDraftHeadword, string CEntryDraftNote, IReadOnlyList<CCardDraft> CEntryDraftMeanings, IReadOnlyList<CCardDraft> CEntryDraftCollocations, CEtymologyDraft CEntryDraftEtymology)`

The held entry, as the editor shows it whenever the desk announces it.
It mirrors only what the editor's parts read, so the full draft stays between controllers.

**Parameters**

- `CEntryDraftHeadword`: the headword.
- `CEntryDraftNote`: the note.
- `CEntryDraftMeanings`: the meaning cards.
- `CEntryDraftCollocations`: the collocation cards.
- `CEntryDraftEtymology`: the etymology.
