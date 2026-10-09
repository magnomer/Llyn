# TDraftCoverageSetting.cs
Hash: `daf81f33186a45ba`

## `internal static class TDraftCoverageSetting`

Hand-written and tracked.
The draft-coverage scope and waivers live here, and no script writes this file.
Each side is a list of path patterns, so a split file joins its side without an edit here.
A waiver is a property name, and the notes below say why each one is waived.

## `public static readonly string[] TDraftTypes`

The records a draft entry is built from, `LForm`, `LInflection` and `LSyllable` included.
`LDraft` is the storage envelope around an entry draft, not a record the entry is built from.

## `public static readonly string[] TDraftWaiver`

Names no side must carry, because none of them is a typed value.

- Every `Id` is a row key, minted by the store.
- `LFormEntryId`, `LInflectionEntryId` and `LSyllablePronunciationId` are the owning row's key.
- Every `Position` is the order of a list, carried by the list itself.
- Every `Seeded` marks a row made unasked, which counts only once filled.
- `LInflectionRegular` is the engine's verdict on the inflection, never typed.
- `LInflectionPrediction`, `LInflectionMarks` and `LInflectionStamp` are the engine's rule-book analysis, never typed.
- `LReflexDraftAnatomy` is cut by the engine from the reading and recut on every change.
- `LReflexDraftAnchors` are fanqie row keys tied by the anchor request.
- `LSpeechDraftCustom` is the typed fallback of a value speech, and every side carries the name instead.

## `public static readonly string[] TDraftPortrait`

The path patterns of the portrait side, in Core, Application and the portrait facade.
A field a draft carries must be read in one of these files, or waived.

## `public static readonly string[] TPortraitWaiver`

Names the portrait shows another way or never prints.

- `LEntryDraftInflections`, `LInflectionMorphology` and `LInflectionSpeechId` reach the paradigm band as stored slots.
- `LSpeechDraftName` and `LSpeechDraftValue` become chips through the vocabulary, not the draft field.
- `LPronunciationDraftAudio` and `LPronunciationDraftSource` are behind the reading, which prints alone.
- `LPronunciationDraftSyllables` and every `LSyllable` part are the cut of a reading the portrait prints whole.
- `LMentionDraftOffset`, `LMentionDraftLength` and `LMentionDraftSense` are the span, and the portrait links the entry.
- `LReflexDraftOwned` marks a user-entered meaning for rebuilds, so the portrait never names it.
- `LSituationDraftDescription` and `LSituationDraftKind` belong to the situation's own page.
  The card shows its title.

## `public static readonly string[] TDraftMarkup`

The path patterns of the markup side, in Core and the four markup clerk files.
A field a draft carries must be read in one of these files, or waived.

## `public static readonly string[] TMarkupWaiver`

- `LSpeechDraftValue` is a pack key, and the markup carries the speech by name.

## `public static readonly string[] TDraftExemplar`

The path of the exemplar side, the test file that fills a whole entry.
A field a draft carries must be named there, or waived.

## `public static readonly string[] TExemplarWaiver`

Empty, since the exemplar fills every typed field by name.
