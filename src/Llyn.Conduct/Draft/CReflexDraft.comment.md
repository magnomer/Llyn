# CReflexDraft.cs
Hash: `020b9e48d15278bf`

## `public sealed record CReflexDraft(`

One reflex of an entry, as the reflex rows show it.

**Parameters**

- `CReflexDraftId`: the stored reflex, zero for a fresh one.
- `CReflexDraftLanguage`: the language the reflex belongs to.
- `CReflexDraftKind`: the kind of reflex.
- `CReflexDraftText`: the reflex's phonetic text.
- `CReflexDraftRespelling`: the reflex respelled, empty when the language has none.
- `CReflexDraftRomanization`: the romanization.
- `CReflexDraftMeaning`: the meaning in that language.
- `CReflexDraftNote`: the note.
- `CReflexDraftMain`: whether the reflex is the language's main one.
- `CReflexDraftRegion`: the region the reflex is heard in.
- `CReflexDraftAnchors`: the fanqie readings the reflex anchors to.
