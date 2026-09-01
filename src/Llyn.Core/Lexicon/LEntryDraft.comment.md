# LEntryDraft.cs

## `public sealed record LEntryDraft(`

The whole input form as one immutable value. The shell reads the visual tree once, builds this, and hands it to the engine; the engine never learns that controls exist and the save never walks the tree. Both card lists are lists of the one card shape, and arrive in the order they are shown: a card's position is its place in its list, not a value it carries.

**Parameters**

- `LEntryDraftHeadword` — Headword text as typed.
- `LEntryDraftLanguage` — Language chosen in the language selector.
- `LEntryDraftPronunciation` — Pronunciation text as typed or filled by a lookup.
- `LEntryDraftNote` — Plain text of the note editor.
- `LEntryDraftSenses` — Meaning cards in list order; their Expression is always empty.
- `LEntryDraftCollocations` — Collocation cards in list order.
- `LEntryDraftAudio` — Full path to the recording downloaded for this entry, or empty when none was chosen. The shell deals in full paths; the engine stores the path relative to the workspace and resolves it back on the way out, so the draft is the same value in both directions.
- `LEntryDraftSource` — Label of the source the recording came from, when there is one.
- `LEntryDraftSpeech` — Part of speech as the field shows it — the display name of a preset the user picked, or whatever they typed instead — and empty when none was given. It travels as text in both directions because that is what the field holds: turning it into the stable id a preset is stored under, or keeping it as text when no preset names it, is the engine's decision and is made once, at the write.
