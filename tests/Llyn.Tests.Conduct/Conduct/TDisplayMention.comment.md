# TDisplayMention.cs
Hash: `c3cfe0e5e95f19c9`

## `public sealed class TDisplayMention`

Covers the reading view's word click gates over a shown entry on a real workspace.
A line names only its sentence row, read from the card gate as the driver reads it.
A sentence naming no language is read in the shown entry's language, and French finds nothing there.
A sole entry opens at once through the Library's leave question and leaves an empty menu.
A declined leave opens nothing.
Several entries stay offered under the found word, and nothing opens or asks.
An astral character before the word moves its UTF-16 unit one past its code-point offset.
That holds when the word starts in a Mention piece before the clicked piece.
A word nobody holds offers an empty menu.
A stored Mention opens its entry and raises its sense.
A row the shown entry does not hold offers an empty menu.
A word of the etymology prose is read in the shown entry's language.
The etymology gate answers nothing while nothing is shown.
A failed find reports `Mention.FindFailed` and offers nothing.

## `private static CDisplay TDisplayMentionPrepare(CAtelier atelier, List<string> asked, long shown)`

Builds a wing whose envoy records into `asked` and shows the entry `shown`.

## `private static List<long> TDisplayLibraryAdd(CAtelier atelier, List<string> asked, bool leave)`

Registers a Library tab whose leave records itself and answers `leave`, and whose arrivals are recorded.

## `private static IReadOnlyList<long> TDisplayLineRead(CDisplay area) =>`

The sentence rows of the shown entry's one Meaning card, in order, as the card gate hands them.

## `private static LSentenceDraft TDisplaySentenceCreate(string text, string language, IReadOnlyList<LMentionDraft> mentions) =>`

A sentence row quoting a new Example with `text`, `language` and `mentions`.

## `private static LEntry TDisplayEntrySave(LEngine engine, string headword, string language, params LSentenceDraft[] sentences) =>`

Saves an entry with one Meaning card under `headword` in `language`, holding `sentences`.
