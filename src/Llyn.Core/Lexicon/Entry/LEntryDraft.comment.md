# LEntryDraft.cs
Hash: `7c3557c606c71b35`

## `public sealed record LEntryDraft(string LEntryDraftHeadword, string LEntryDraftLanguage, IReadOnlyList<LPronunciationDraft>? LEntryDraftPronunciations, string LEntryDraftNote, IReadOnlyList<LCardDraft> LEntryDraftMeanings, IReadOnlyList<LCardDraft> LEntryDraftCollocations, IReadOnlyList<LSpeechDraft>? LEntryDraftSpeeches = null, IReadOnlyList<LForm>? LEntryDraftForms = null, IReadOnlyList<LInflection>? LEntryDraftInflections = null, IReadOnlyList<LTranscriptionDraft>? LEntryDraftTranscriptions = null, IReadOnlyList<LReflexDraft>? LEntryDraftReflexes = null, LEtymologyDraft? LEntryDraftEtymology = null)`

The whole input form as one immutable value.
The shell reads the visual tree once, builds this, and hands it to the engine.
The engine never learns that controls exist and the save never walks the tree.
Both card lists are lists of the one card shape, and arrive in the order they are shown.
A card's position is its place in its list, not a value it carries.
The entry-level detail travels as the shapes the archives store, never as flattened text.

**Parameters**

- `LEntryDraftHeadword` — Headword text as typed.
- `LEntryDraftLanguage` — Language chosen in the language selector.
- `LEntryDraftPronunciations` — Every pronunciation in list order, the first being the primary one.
  Empty when the entry records none.
- `LEntryDraftNote` — Markdown text of the note editor.
- `LEntryDraftMeanings` — Meaning cards in list order, whose Expression is always empty.
- `LEntryDraftCollocations` — Collocation cards in list order.
- `LEntryDraftSpeeches` — Parts of speech in order, each a language-pack value or a typed name.
- `LEntryDraftForms` — Variant forms in order, each with its role and its localized label.
- `LEntryDraftInflections` — Inflections in order, each owning its ordered features.
- `LEntryDraftTranscriptions` — Transcriptions in list order, one per scheme.
  Empty when the language shows none.
- `LEntryDraftReflexes` — Reflexes in list order, grouped by borrowing language.
  Empty when the language declares no reflex rule or nothing was fetched or typed.
- `LEntryDraftEtymology` — What the entry says about its own origin, in either shape.
  Never null, since an entry that says nothing carries an empty etymology draft.

## `public IReadOnlyList<string> LEntryDraftNames`

The names of the parts of speech a reader reads, in order.
A part stored with no name is left out, since only its stored value would show.

## `public bool LEntryDraftDerived`

Whether the entry says anything about its origin, by prose or by link.

## `public IReadOnlyList<long> LEntryDraftSources`

Every entry the etymology names, its source links and the spans of its narrative alike.
One list lets a reader resolve them all in a single query.

## `public IReadOnlyList<long> LEntryDraftCitations`

Every Source an Example of the entry cites, across meanings, collocations and their child cards.
The display reads one line for each of them in a single query.

## `public LPronunciationDraft? LEntryDraftPronunciation`

The primary pronunciation alone, the first of the list, or null when there is none.
The surfaces that show one reading read this one.

## `public string LEntryDraftIpa`

The primary reading's IPA alone, for the surfaces that show only that.

## `public string LEntryDraftAudio`

The primary reading's recording path alone, for the surfaces that play only that.

## `public IReadOnlyList<LPronunciationDraft> LEntryDraftAccents`

Every pronunciation after the primary one, in list order, empty when the entry has one or none.
The accent sheet and the recording checks read these beside the primary reading.

## `public LEntryDraft LEntryDraftNormalize()`

The same draft, across every meaning and collocation card with every unreadable value dropped to unspecified.
Called only after the user agreed to lose what the store could not read.
