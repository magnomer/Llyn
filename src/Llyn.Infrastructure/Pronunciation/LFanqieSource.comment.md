# LFanqieSource.cs

## `public static class LFanqieSource`

Asks one rime book's site for one character and reads the placements it answers with.
A table site answers with the whole rime table the character sits in, one table per rime group.
A cell there holds one group per 小韻, and only the groups carrying the marked character are read.
A line site answers with plain text, one placement per match of the book's line pattern.
Nothing here knows the site: the address, the form and every pattern come from the pack.

## `private const string LFanqieSourceToken = "{word}";`

The placeholder in a form value that the character replaces.

## `private const RegexOptions LFanqieSourceLoose =`

The options the table patterns share: case-blind tags and dots that cross lines.

## `private static readonly Regex LFanqieSourceTable = new("<table[^>]*>(.*?)</table>", LFanqieSourceLoose);`

One rime table, whose rows are read in order.

## `private static readonly Regex LFanqieSourceLine = new("<tr[^>]*>(.*?)</tr>", LFanqieSourceLoose);`

One row of a table, either the heading row or a row of one initial.

## `private static readonly Regex LFanqieSourceHead = new("<th[^>]*>(.*?)</th>", LFanqieSourceLoose);`

One heading cell, naming the division and tone of the column beneath it.

## `private static readonly Regex LFanqieSourceCell = new("<td[^>]*>(.*?)</td>", LFanqieSourceLoose);`

One body cell, the first of a row naming the initial and the rest holding groups of characters.

## `private static readonly Regex LFanqieSourceStrike = new("<s(?:\\s[^>]*)?>.*?</s>", LFanqieSourceLoose);`

The struck-through division digit the site prints inside a group, dropped from the line since the heading says it.

## `private static readonly Regex LFanqieSourceAnchor = new("<a[\\s>]", LFanqieSourceLoose);`

The first link in a group, which starts the fanqie spelling and the character list after it.
The group's head, the rime and its rime heading, is everything before it.

## `private static readonly Regex LFanqieSourceTag = new("<[^>]+>", RegexOptions.CultureInvariant);`

Any tag, dropped when a cell is read into text.

## `private static readonly TimeSpan LFanqieSourcePatience = TimeSpan.FromSeconds(2);`

How long the busy pattern may run over one answer before it is given up.

## `public static async Task<(IReadOnlyList<LFanqieRow> LFanqieFound, bool LFanqieReached)> LFanqieSourceFind(`

Fetches the answer and reads every placement of the character, in answer order.
The table reader or the line reader is chosen by whether the book has a line pattern.
An unreachable site or a failed request reads as not reached, with nothing found.
An answer matching the book's busy pattern is a throttle, so it too reads as not reached.
A pattern that runs past its patience reads as reached with nothing found.

## `private static async Task<string?> LFanqieBodyRead(`

Posts the book's form with the character in place of the token and returns the answer's text.
A book with no form field is fetched with a GET, the character escaped into the address.
A not-found status returns an empty answer, since the site did reply and has nothing for the character.
Any other refused status or a transport failure returns `null`.
A cancellation asked for by the caller is thrown through, since the fetch is being abandoned.

## `private static IReadOnlyList<LFanqieRow> LFanqieOriginSet(`

Sets the character, book, source and position on the rows a reader left blank.
The positions count across the answer.

## `private static IReadOnlyList<LFanqieRow> LFanqieMatchScan(LFanqieShape shape, string body)`

One row per match of the line pattern over the whole answer, its named parts read into the row.
The `spelling` part, when the pattern names one, is the 反切.
The line text joins the initial, the rime with its heading, and the division and tone.
The rounded part counts when the book's rounded pattern matches it.

## `private static IReadOnlyList<LFanqieRow> LFanqieTableScan(LFanqieShape shape, string body)`

Walks every table and every row, keeping the last heading row read as the columns.
Each marked group of a body row becomes one row.

## `private static LFanqieColumn LFanqieColumnRead(LFanqieShape shape, string text)`

One column heading with its division and tone read by the column pattern, or empty parts without it.

## `private static IEnumerable<LFanqieRow> LFanqieLineScan(`

Reads one body row: the first cell is the initial.
Every marked group of a later cell yields a row.
A cell past the last column is read under an empty column.

## `private static IEnumerable<string> LFanqieGroupScan(LFanqieShape shape, string cell)`

The groups of a cell, parted by the split pattern, or the whole cell when the book has none.

## `private static LFanqieRow LFanqieGroupRead(LFanqieShape shape, string initial, LFanqieColumn column, string group)`

One row from one marked group: the line text, the parts the hypothesis reads and the 反切 spelling.
The column's division wins over the group's, since the group's follows the 韻鏡 placement.
The character, book, source and position are left blank here and set afterwards.

## `private static IEnumerable<string> LFanqiePartScan(params string[] parts)`

The parts of a line that are not empty, so a missing heading leaves no double space.

## `private static string LFanqieCellRead(string cell)`

The rime and its rime heading printed at the head of a group, before the first link.
The struck-through division is dropped first.

## `private static string LFanqieTextNormalize(string raw)`

Drops tags, decodes entities and collapses runs of whitespace to one space.
