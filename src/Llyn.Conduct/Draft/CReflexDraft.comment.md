# CReflexDraft.cs
Hash: `6d3c5be31f5d104d`

## `public sealed record CReflexDraft(long CReflexDraftId, string CReflexDraftLanguage, string CReflexDraftKind, string CReflexDraftRomanization, string CReflexDraftMeaning, string CReflexDraftNote, bool CReflexDraftMain, string CReflexDraftRegion, IReadOnlyList<long> CReflexDraftAnchors)`

One reflex of an entry, as the reflex rows show it.
Its shown reading comes from the row's guise, not from this shape.

**Parameters**

- `CReflexDraftId`: the stored reflex, zero for a fresh one.
- `CReflexDraftLanguage`: the language the reflex belongs to.
- `CReflexDraftKind`: the kind of reflex.
- `CReflexDraftRomanization`: the romanization.
- `CReflexDraftMeaning`: the meaning in that language.
- `CReflexDraftNote`: the note.
- `CReflexDraftMain`: whether the reflex is the language's main one.
- `CReflexDraftRegion`: the region the reflex is heard in.
- `CReflexDraftAnchors`: the fanqie readings the reflex anchors to.

## `internal static IReadOnlyList<CReflexDraft> CReflexDraftRead(IReadOnlyList<LReflexDraft> reflexes)`

The reflexes of a draft, shaped for the reflex rows and the lectern.

## `private static CReflexDraft CReflexDraftRead(LReflexDraft reflex)`

Copies the fields the rows read from one engine reflex draft into its shape.
