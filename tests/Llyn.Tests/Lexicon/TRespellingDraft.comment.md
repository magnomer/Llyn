# TRespellingDraft.cs

## `public sealed class TRespellingDraft`

Covers the respelling stored beside every reading and the switch that picks which one is shown.
A reading request derives the row's respelling through the draft language's pack, whatever the switch says.
A respelling request overwrites that form alone, and the next reading or variety request derives it again.
A variety request derives again because the English groups are scoped by variety.
A language request derives every row again, blank under a pack without groups.
The two forms are stored in their own columns and read back into the draft.
A respelling-only edit counts as a change.
The check answers true only while the switch is on and the pack declares groups.
The phonemic flag is the pack's alone.
Flipping the switch raises a settings bulletin so open readings redraw.
A reflex row's text derives a respelling through the row's own language, slashed for a phonemic pack.
A reflex respelling written by hand stands until the text or the language changes.
Rows saved without a respelling, pronunciation and reflex alike, have one derived and stored on the way in.
A load reads the stored form back and derives nothing.

## `private sealed class TRespellingObserver : LObserver`

Records every bulletin subject the engine raises, so a flip of the switch can be seen.

## `private static LEntryDraft TRespellingDraftCreate(IReadOnlyList<LPronunciationDraft> pronunciations)`

A Mandarin entry draft carrying the given pronunciation rows and one meaning.
