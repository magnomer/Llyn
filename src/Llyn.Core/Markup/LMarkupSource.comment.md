# LMarkupSource.cs

## `public static class LMarkupSource`

Writes one source block with its credited authors.

## `public static void LMarkupSourceAppend(StringBuilder text, int depth, string key, LMarkup.LMarkupReference source)`

The key is written as an attribute, because an example cites a source by that key alone.
The key is passed in rather than read off the row, because only the document decides what a row is called.

Authors are written in credited order, which is the order the reader restores.
Each is a `ref` naming an author the catalog declares, never a name written twice.
Two authors sharing a name are two rows, and only their keys keep them apart.

A source whose credit is unreadable writes `<author/>` with no `ref`, as section 3 requires.
A source that credits no one writes no `<author>` at all.
The three cases are the same three states every other field carries.

**Parameters**

- `text` — The document being written.
- `depth` — How far the block is indented, counted in two-space steps.
- `key` — The key this source is declared under.
- `source` — The stored source and the keys of the authors it credits.
