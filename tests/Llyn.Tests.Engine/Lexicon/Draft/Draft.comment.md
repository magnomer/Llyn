# Draft

This folder holds the tests of draft coverage.
They also cover the facts a draft answers about itself.

## `TDraftCoverage`

Keeps every draft field on every side that carries a draft.
A new record field must be named by the portrait, the markup and the exemplar, or waived by name.
Naming is a word match over the side's sources, so a positional argument never counts.
The settings and every waiver reason live in `TDraftCoverageSetting`.

The portrait side is the `Portrait` folders of the core and the application, plus `LCollocation.cs` and the engine's portrait facade.
The markup side is the `Markup` folder of the core and the application's markup clerk.
The exemplar side is the two files that build the shared exemplar entry and list its typed strings.
Every listed type must be a record in the core, and every shared waiver must still be a property.

### `TDraftCheck`

A setting that lists no draft type has nothing to hold, so the check returns at once.
A shared waiver is skipped outright.
A side waiver that the side names anyway is stale, as is one naming no property.
The report lists the unnamed properties by record, then the stale waiver lines.

### `TDraftRead`

Reads the listed records from the loaded core assembly, so the compiled shape is the truth.
A type counts as a record only when it carries the compiler's equality contract.
Only primary constructor parameters count, so a derived property such as `LCardDraftEmpty` never enters the test.
The copy constructor is skipped, since its one parameter is the record itself.

### `TSourceFind`

A pattern with a star searches its folder and every subfolder, as a git pathspec star also crosses `/`.
A pattern without a star names one file.
Build output under `bin` or `obj` is never a side.
The files come back in ordinal path order, so the joined text is stable.

### `TSourceRead`

Joins every matched file, and refuses an empty match as a stale pattern.

### `TSourceResolve`

Walks up from the test binaries to the folder holding `Llyn.slnx`, since the sides are read as sources.

## `TDraftCoverageSetting`

Hand-written and tracked, and no script writes it.
The draft-coverage scope and waivers live here.
Each side is a list of path patterns.
A star pattern takes a whole folder, so a file split inside it joins its side without an edit here.
A waiver is a property name, and the notes below say why each one is waived.

### `TDraftTypes`

The records a draft entry is built from, `LForm`, `LInflection` and `LSyllable` included.
`LDraft` is the storage envelope around an entry draft, not a record the entry is built from.

### `TDraftWaiver`

These names no side must carry, because none of them is a typed value.

- Every `Id` is a row key, minted by the store.
  `LAncestorDraftId` is the exception, since the markup clerk maps it to the box key it writes.
- `LFormEntryId`, `LInflectionEntryId` and `LSyllablePronunciationId` are the owning row's key.
- Every `Position` is the order of a list, carried by the list itself.
- Every `Seeded` marks a row made unasked, which counts only once filled.
- `LInflectionRegular` is the engine's verdict on the inflection, never typed.
- `LInflectionPrediction`, `LInflectionMarks` and `LInflectionStamp` are the engine's rule-book analysis, never typed.
- `LReflexDraftAnatomy` is cut by the engine from the reading and recut on every change.
- `LReflexDraftAnchors` are fanqie row keys tied by the anchor request.
- `LSpeechDraftCustom` is the typed fallback of a value speech, and every side carries the name instead.

### `TDraftPortrait`

The path patterns of the portrait side, in Core, Application and the portrait facade.
`LCollocation.cs` joins the side because the portrait reads a collocation's meaning links through it.
Its first link that still exists is the home the collocation prints under, and every further one gets a see-line.

### `TPortraitWaiver`

These are names the portrait shows another way or never prints.

- `LEntryDraftInflections`, `LInflectionMorphology` and `LInflectionSpeechId` reach the paradigm band as stored slots.
- `LSpeechDraftName` and `LSpeechDraftValue` become chips through the vocabulary, not the draft field.
- `LPronunciationDraftAudio` and `LPronunciationDraftSource` are behind the reading, which prints alone.
- `LPronunciationDraftSyllables` and every `LSyllable` part are the cut of a reading the portrait prints whole.
- `LMentionDraftOffset`, `LMentionDraftLength` and `LMentionDraftSense` are the span, and the portrait links the entry.
- Every `LAncestorDraft` part reaches the portrait through the lineage clerk, which lives outside the side.
- `LReflexDraftOwned` marks a user-entered meaning for rebuilds, so the portrait never names it.
- `LSituationDraftDescription` and `LSituationDraftKind` belong to the situation's own page.
  The card shows its title.

### `TDraftMarkup`

The path patterns of the markup side, in Core and the five markup clerk files.

### `TMarkupWaiver`

`LSpeechDraftValue` is a pack key, and the markup carries the speech by name.

### `TDraftExemplar`

The paths of the exemplar side, the test file that fills a whole entry and the one listing its strings.

### `TExemplarWaiver`

The exemplar passes an Ancestor box key by position.
Its two boxes leave `LAncestorDraftChild`, `LAncestorDraftRelation` and `LAncestorDraftTongue` unset.
So these four names never appear in the exemplar side.

## `TDraftFact`

Holds the facts a draft and the values in it answer about themselves.
So the shell copies a verdict instead of counting.

## Stamp

Commit: `c6e60141a172`
- `TDraftCoverage.cs` `9dba35a561f37211`
- `TDraftCoverageSetting.cs` `1d6eff7d69841857`
- `TDraftFact.cs` `faf0693926ac304b`
