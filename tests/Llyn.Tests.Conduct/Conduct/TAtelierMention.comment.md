# TAtelierMention.cs
Hash: `f5b5f7bfd1efaa25`

## `public sealed class TAtelierMention`

Covers the mention gates over fake ports that answer with the real span rules.
A linked Mention divides the text into three pieces, and only the linked one is marked.
A selection and a caret convert between code points and UTF-16 units both ways.
A piece answers the unit of an offset counted from its own start, and nothing for an offset outside it.
A click result carries the stored Mention's entry and sense, and a click on nothing stored carries no mark.
A click on a stored Mention opens its entry and then raises its sense, if it names one.
An unlinked Mention or a declined leave opens nothing and raises no sense.
A click on one candidate opens it.
A click on several candidates or none opens nothing and offers the list.
A result with one target names its id, and a result with several names none.

## `private static List<long> TMentionLibraryAdd(CAtelier atelier, bool leave)`

Registers a Library tab whose leave answers `leave` and whose landings are recorded.

## `private static CAtelier TAtelierMentionCreate(LEngine engine)`

Builds the atelier whose span calls run the real span rules.

## `private static CTranslationTarget TMentionTargetCreate(long id)`

Builds one translation target with the given id.
