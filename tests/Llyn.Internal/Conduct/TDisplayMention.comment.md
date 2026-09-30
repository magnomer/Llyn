# TDisplayMention.cs

## `public sealed class TDisplayMention`

Covers the reading view's word click gate over a shown entry on a real workspace.
A text naming no language is read in the shown entry's language, and French finds nothing there.
A sole entry opens at once through the Library's leave question and leaves an empty menu.
A declined leave opens nothing.
Several entries stay offered under the found word, and nothing opens or asks.
A word nobody holds offers an empty menu.
A stored Mention opens its entry and raises its sense.
A failed find reports `Mention.FindFailed` and offers nothing.

## `private static CDisplay TDisplayMentionPrepare(CAtelier atelier, List<string> asked, long shown)`

Builds a wing whose envoy records into `asked` and shows the entry `shown`.

## `private static List<long> TDisplayLibraryAdd(CAtelier atelier, List<string> asked, bool leave)`

Registers a Library tab whose leave records itself and answers `leave`, and whose arrivals are recorded.

## `private static LEntry TDisplayEntrySave(LEngine engine, string headword, string language) =>`

Saves an entry with one Meaning card under `headword` in `language`.
