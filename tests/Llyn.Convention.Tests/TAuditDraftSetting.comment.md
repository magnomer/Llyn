# TAuditDraftSetting.cs

## `internal static class TAuditDraftSetting`

Hand-written and tracked: the draft-coverage scope and waivers live here, and no script writes this file.
Each side is a list of `git ls-files` patterns, so a split file joins its side without an edit here.
A waiver is a property name, and this file names why each one is waived.

## `public static readonly string[] TAuditDraftTypes`

The records a draft entry is built from, `LForm`, `LInflection` and `LSyllable` included.
`LDraft` is the storage envelope around an entry draft and carries no typed field of its own.

## `public static readonly string[] TAuditDraftWaiver`

Names no side must carry, because none of them is a typed value.

- Every `Id` is a row key, minted by the store and stripped on load.
- `LFormEntryId`, `LInflectionEntryId` and `LSyllablePronunciationId` are the owning row's key.
- Every `Position` is the order of a list, carried by the list itself.
- Every `Seeded` marks a row the pack wrote, which no side may claim.
- `LInflectionRegular` is the engine's verdict on the inflection, never typed.
- `LReflexDraftAnatomy` is cut by the engine from the reading and recut on every change.
- `LReflexDraftAnchors` are fanqie row keys tied by the anchor request.
- `LSpeechDraftCustom` is the typed fallback of a value speech, and every side carries the name instead.

## `public static readonly string[] TAuditPortraitWaiver`

Names the portrait shows another way or never prints.

- `LEntryDraftInflections`, `LInflectionMorphology` and `LInflectionSpeechId` reach the paradigm band as stored slots.
- `LSpeechDraftName` and `LSpeechDraftValue` become chips through the vocabulary, not the draft field.
- `LPronunciationDraftAudio` and `LPronunciationDraftSource` are behind the reading, which prints alone.
- `LPronunciationDraftSyllables` and every `LSyllable` part are the cut of a reading the portrait prints whole.
- `LMentionDraftOffset`, `LMentionDraftLength` and `LMentionDraftSense` are the span, and the portrait links the entry.
- `LSituationDraftDescription` and `LSituationDraftKind` belong to the situation's own page, the card shows its title.

## `public static readonly string[] TAuditMarkupWaiver`

- `LSpeechDraftValue` is a pack key, and the markup carries the speech by name.

## `public static readonly string[] TAuditExemplarWaiver`

Empty: the exemplar fills every typed field by name.
