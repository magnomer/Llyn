# Fanqie

This folder holds the web adapter that fetches rime-book placements.
It also holds the patterns and column headings it reads them with.

## `LFanqieSourceHttp`

The adapter behind the `LFanqieSource` port.
It asks one rime book's site for one character and reads the placements it answers with.
A table site answers with the whole rime table the character sits in, one table per rime group.
A cell there holds one group per 小韻, and only groups carrying the marked character are read.
A line site answers with plain text, one placement per match of the book's line pattern.
The address, the form and every pattern come from the pack.

### `LFanqieSourceFind`

Fetches the answer and reads every placement of the character, in answer order.
The table or line reader is chosen by whether the book has a line pattern.
An unreachable site or a failed request reads as not reached, with nothing found.
An answer matching the book's busy pattern is a throttle, so it too reads as not reached.
A pattern running past its patience reads as not reached, since the answer was never fully read.

### `LFanqieBodyRead`

Posts the book's form with the character in place of the token and returns the answer's text.
A book with no form field is fetched with a GET, the character escaped into the address.
A not-found status returns an empty answer, since the site did reply and has nothing for the character.
Any other refused status or a transport failure returns `null`.
A cancellation asked for by the caller is thrown through, since the fetch is being abandoned.

### `LFanqieOriginSet`

Sets the character, book, source and position on the rows a reader left blank.
The positions count across the answer.

### `LFanqieMatchScan`

One row per match of the line pattern over the whole answer, its named parts read into the row.
The `spelling` part, when named, is the 反切.
The line text joins the initial, the rime with its heading, and the division and tone.
The rounded part counts when the book's rounded pattern matches it.

### `LFanqieTableScan`

Walks every table and row, keeping the last heading row read as the columns.
Each marked group of a body row becomes one row.

### `LFanqieColumnRead`

Reads one column heading's division and tone by the column pattern, or leaves them empty without it.

### `LFanqieLineScan`

Reads one body row, whose first cell is the initial.
Every marked group of a later cell yields a row.
A cell past the last column is read under an empty column.

### `LFanqieGroupScan`

The groups of a cell, parted by the split pattern, or the whole cell when the book has none.

### `LFanqieGroupRead`

One row from one marked group, carrying the line text, the parts the hypothesis reads and the 反切 spelling.
The column's division wins over the group's, since the group's follows the 韻鏡 placement.
The character, book, source and position are left blank here and set afterwards.

### `LFanqiePartScan`

Keeps the non-empty parts of a line, so a missing heading leaves no double space.

### `LFanqieCellRead`

The rime and its rime heading printed at the head of a group, before the first link.
The struck-through division is dropped first, since the column heading already says it.

### `LFanqieTextNormalize`

Drops tags, decodes entities and collapses runs of whitespace to one space.

### Table regexes

The table patterns share options that make tags case-blind and let dots cross lines.
They split an answer into `table`, `tr`, `th` and `td` pieces.
The first `td` of a row names the initial and the rest hold groups.
The anchor pattern finds the first link in a group, which starts the fanqie spelling and character list.
The group's head is everything before it.
The strike pattern drops the struck-through division digit the site prints inside a group.
The adapter's patience bounds the busy pattern over one answer.

## `LFanqieShape`

The compiled patterns of one book for one character, built once per answer.
Every pattern comes from the pack row, with the character in place of the `{word}` token, escaped for the regex.
Any one pattern may run for the patience the caller passes in before it is given up.
`LFanqieShapeCreate` requires the marker from the loader.
Every other pattern compiles to `null` when the pack omits it.

- `LFanqieShapeMarker` marks the queried character inside a group, so only groups carrying it are read.
- `LFanqieShapeSplit` parts a cell into groups, or is `null` when a cell is one group.
- `LFanqieShapeHead` reads the named parts `rime`, `heading` and `division`, or is `null` to keep the head as text only.
- `LFanqieShapeColumn` reads the parts `division` and `tone` from a column heading.
- `LFanqieShapeRounded` means the marked character is rounded, or is `null` when the book does not say.
- `LFanqieShapeLine` reads one placement per match over the whole answer, or is `null` when the book is read as tables.
- `LFanqieShapeSpelling` reads the 反切 spelling as the first group of a table group, or is `null` when none is printed.

Dots cross lines in every compiled pattern.

## `LFanqieColumn`

One column heading of a rime table, as printed and as read apart.
Its text, such as `一等平`, is appended to every line under it.
Its division and tone come from the column pattern, and are empty without it.

## Stamp

Commit: `9377e657b395`
- `LFanqieColumn.cs` `fe092d15c9a6224c`
- `LFanqieShape.cs` `da5256c124724a34`
- `LFanqieSourceHttp.cs` `15e2b774ce9a94c6`
