# LLanguageLoaderFanqie.cs

## `public static partial class LLanguageLoader`

The `fanqie` side of the pack loader: the rime books a Han language pack lists.
Each row becomes an [LFanqieBook](../../Llyn.Core/Pronunciation/LFanqieBook.comment.md), in written order.

## `private static IReadOnlyList<LFanqieBook> LLanguageFanqieScan(JsonElement root)`

Reads every row of the `fanqie` array, skipping rows the book reader refuses.
A pack without the array yields an empty list, and the reading view shows no fanqie box.

## `private static LFanqieBook? LLanguageFanqieRead(JsonElement row)`

Reads one book row.
A row without a name, a URL or a match pattern is refused, since nothing could be posted or read.
The form fields are read by the script loader's form reader, since both post a search form.
The busy pattern and the interval are optional and read as `null` and zero.
The split, head, column, rounded, line and spelling patterns are optional too and read as `null`.
The source label is optional and falls back to the book name in the record.
