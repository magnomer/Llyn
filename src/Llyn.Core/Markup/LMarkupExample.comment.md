# LMarkupExample.cs

## `public static class LMarkupExample`

Writes the two halves of a quoted sentence: the catalog row, and the card's use of it.
They sit in one file because a use is meaningless without the row it names.

## `public static void LMarkupExampleAppend(StringBuilder text, int depth, LSentenceDraft sentence, IReadOnlyDictionary<string, string> keys)`

Writes one `<use>`: the card's position that may quote an example and may state a frame.

A use holding neither is not written, because the reader refuses one on the way back in.
A use quoting an example writes `ref` naming the catalog key for that row.
A use whose example is not in this document writes `ref=""`, which quotes an unreadable example.
That is the honest shape: the card quotes something, and the file cannot say what.

`par` and `dep` follow the three-state rule of section 6.
An absent frame writes no attribute, and an unreadable marker writes `par=""`.

**Parameters**

- `text` — The document being written.
- `depth` — How far the use is indented, counted in two-space steps.
- `sentence` — The use to write.
- `keys` — Every stored row id, and the document key it is written under.

## `public static void LMarkupExampleAppend(StringBuilder text, int depth, string key, LExample example)`

Writes one `<example>` row of the catalog.

The key is written as an attribute, because a use names an example by that key alone.
`lang` names the language of the sentence and is plain text, so an empty language writes no attribute.
`src` cites the source and is three-state, so an unreadable citation writes `src=""`.
The citation already holds a document key rather than a stored id when it reaches here.

`<text>` and `<trans>` follow the three-state rule and are omitted when never written.
