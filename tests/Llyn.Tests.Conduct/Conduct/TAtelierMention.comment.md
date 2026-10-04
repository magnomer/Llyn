# TAtelierMention.cs
Hash: `3af14e590ada7db8`

## `public sealed class TAtelierMention`

Covers the mention gates, mostly over fake ports that answer with the real span rules.
A linked Mention divides the text into three pieces, and only the linked one is marked.
A code-point offset converts to a UTF-16 unit and back through the display find gate.
Through the display find gate, two units past the astral character reach the engine two code points on.
A piece answers the unit of an offset counted from its own start, and nothing for an offset outside it.
A click result carries the stored Mention's entry and sense, and a click on nothing stored carries no mark.
A click on a stored Mention opens its entry and then raises its sense, if it names one.
An unlinked Mention or a declined leave opens nothing and raises no sense.
A click on one candidate opens it.
A click on several candidates or none opens nothing and offers the list.
Every open places the offer on the found word, as its UTF-16 unit in the whole text.
A result with one target names its id, and a result with several names none.

## `private static List<long> TMentionLibraryAdd(CAtelier atelier, bool leave)`

Registers a Library tab whose leave answers `leave` and whose landings are recorded.

## `private static CAtelier TAtelierMentionCreate(LEngine engine)`

Builds the atelier whose span calls run the real span rules.

## `private static CTranslationTarget TMentionTargetCreate(long id)`

Builds one translation target with the given id.
