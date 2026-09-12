# LEntryDraft.cs

## `public sealed record LEntryDraft(`

The whole input form as one immutable value.
The shell reads the visual tree once, builds this, and hands it to the engine.
The engine never learns that controls exist and the save never walks the tree.
Both card lists are lists of the one card shape, and arrive in the order they are shown.
A card's position is its place in its list, not a value it carries.
The entry-level detail travels as the shapes the archives store, never as flattened text.

**Parameters**

- `LEntryDraftHeadword` — Headword text as typed.
- `LEntryDraftLanguage` — Language chosen in the language selector.
- `LEntryDraftPronunciations` — Every pronunciation in list order, the first being the primary one; empty when the entry records none.
- `LEntryDraftNote` — Markdown text of the note editor.
- `LEntryDraftMeanings` — Meaning cards in list order, whose Expression is always empty.
- `LEntryDraftCollocations` — Collocation cards in list order.
- `LEntryDraftSpeeches` — Parts of speech in order, each a language-pack value or a typed name.
- `LEntryDraftForms` — Variant forms in order, each with its role and its localized label.
- `LEntryDraftInflections` — Inflections in order, each owning its ordered features.
- `LEntryDraftTranscriptions` — Transcriptions in list order, one per scheme; empty when the language shows none.

## `public LPronunciationDraft? LEntryDraftPronunciation`

The primary pronunciation alone, the first of the list, or null when there is none.
The surfaces that show one reading read this one.

## `public string LEntryDraftIpa`

The primary reading's IPA alone, for the surfaces that show only that.

## `public string LEntryDraftAudio`

The primary reading's recording path alone, for the surfaces that play only that.
