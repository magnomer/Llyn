# TAtelierMention.cs

## `public sealed class TAtelierMention`

Covers the mention gates over fake ports that answer with the real span rules.
A linked Mention divides the text into three pieces, and only the linked one is marked.
An unlinked chip takes the silent label the driver hands in.
A selection and a caret convert between code points and UTF-16 units both ways.
A draft's Mentions resolve into stored marks, and no draft list reads none.
A click result carries the stored Mention's entry and sense, and a click on nothing stored carries no mark.
A click on a stored Mention opens its entry, and its sense is raised only after the open went through.
A click on one candidate opens it, and a click on several offers them and opens nothing.

## `private static List<long> TMentionLibraryAdd(CAtelier atelier, bool leave)`

Registers a Library tab whose leave answers `leave` and whose landings are recorded.

## `private static CAtelier TAtelierMentionCreate(LEngine engine, IReadOnlyList<LMentionLabel> labels)`

Builds the atelier whose resolve answers `labels` and whose span calls run the real span rules.
