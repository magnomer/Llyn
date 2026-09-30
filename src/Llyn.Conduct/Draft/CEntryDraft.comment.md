# CEntryDraft.cs

## `public sealed record CEntryDraft(`

The held entry, as the editor shows it whenever the desk announces it.
It mirrors only what the editor's parts read, so the full draft stays between controllers.

**Parameters**

- `CEntryDraftHeadword`: the headword.
- `CEntryDraftLanguage`: the entry's language.
- `CEntryDraftNote`: the note.
- `CEntryDraftMeanings`: the meaning cards.
- `CEntryDraftCollocations`: the collocation cards.
- `CEntryDraftTranscriptions`: the transcriptions.
- `CEntryDraftReflexes`: the reflexes.
- `CEntryDraftEtymology`: the etymology.

## `public bool CEntryDraftReflected`

Whether the entry holds any reflex, so the editor prepares the reflex panel.
