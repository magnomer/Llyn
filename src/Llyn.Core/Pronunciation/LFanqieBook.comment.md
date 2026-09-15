# LFanqieBook.cs

## `public sealed record LFanqieBook(`

One rime book a language pack lists under `fanqie`, such as 廣韻 or 集韻.
The book names the web database that places a character in its rime tables and how its answer is read.
The engine knows nothing of the site, only that a form is posted and a table is read.
Every URL, field and pattern is pack data, so a new book needs no code.

**Parameters**

- `LFanqieBookName` — The book's name, shown as the block's chip and stored beside every row of it.
- `LFanqieBookUrl` — The address the search form is posted to.
- `LFanqieBookForm` — The form fields posted, in pack order, with `{word}` replaced by the character.
  A book with no field is fetched with a GET instead, the character escaped into the address.
- `LFanqieBookPattern` — A .NET regex marking the queried character inside a table cell, with `{word}` replaced.
  The site wraps the character it was asked for in a marker, and only cells carrying it are read.
- `LFanqieBookBusy` — A regex matched over the answer that means the site refused the request, or `null`.
  A refusal is a throttle, not a miss, so the character is asked again later.
- `LFanqieBookInterval` — Seconds the engine waits after one post before the next to any book.
  The site refuses a request that follows another within a few seconds.
- `LFanqieBookSplit` — A regex splitting one cell into its groups, each with its own head, or `null` for one group.
  A cell of the 廣韻 tables holds one group per 小韻, parted by a line break.
- `LFanqieBookHead` — A regex over a group reading its named parts `rime`, `heading` and `division`, or `null`.
  Without it the line keeps the group's head as text and the parts stay empty.
- `LFanqieBookColumn` — A regex over a column heading reading its named parts `division` and `tone`, or `null`.
  The column's division wins over the group's, since the group's follows the 韻鏡 placement.
- `LFanqieBookRounded` — A regex matched over a group that means the marked character is rounded, or `null`.
  The 廣韻 tables underline a rounded character, and a line reader captures 合 into its `rounded` part.
- `LFanqieBookSource` — The site the book is read from, shown as the chip at the right of its lines.
  Two rows may name the same book from two sites, and each keeps its own lines under its own chip.
  A row without one is labelled by its book name.
- `LFanqieBookLine` — A regex over the whole answer reading one placement per match, or `null` for the table reader.
  Its named parts are `initial`, `rime`, `heading`, `division`, `tone`, `rounded` and `spelling`, any of them optional.
  A site that answers in plain text, one reading per line, is read this way.
- `LFanqieBookSpelling` — A regex over a table group whose first group is the 反切 spelling, or `null`.
  The 廣韻 tables print it as the link that opens the 小韻.
