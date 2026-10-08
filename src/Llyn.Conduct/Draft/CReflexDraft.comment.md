# CReflexDraft.cs
Hash: `4716820364a453aa`

## `public sealed record CReflexDraft(long CReflexDraftId, string CReflexDraftLanguage, string CReflexDraftKind, string CReflexDraftText, string CReflexDraftRespelling, string CReflexDraftRomanization, string CReflexDraftMeaning, string CReflexDraftNote, bool CReflexDraftMain, string CReflexDraftRegion, IReadOnlyList<long> CReflexDraftAnchors)`

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

## `internal static IReadOnlyList<CReflexDraft> CReflexDraftRead(IReadOnlyList<LReflexDraft> reflexes)`

The reflexes of a draft, shaped for the reflex rows and the lectern.

## `private static CReflexDraft CReflexDraftRead(LReflexDraft reflex)`

Copies every field of one engine reflex draft into its shape.
