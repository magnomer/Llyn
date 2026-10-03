# TTranscriptionSheet.cs
Hash: `7f46294903369593`

## `public sealed class TTranscriptionSheet`

Covers the editor's transcription block and its row gates end to end on a real workspace.

The read marks each row's dropdown by the one-scheme rule and answers whether a row can be added.
A language without schemes hides the block but keeps its rows, and an empty desk answers nothing.
A plus takes the first free scheme, after the pressed row or at the end.
A glyph row before the pressed row does not shift the place.
It stops once no scheme is free.
A scheme pick switches the row, and a pick another row holds is refused.
A minus drops the row and frees its scheme.
A filling desk writes nothing for a plus, a scheme pick or a minus.

## `private static IReadOnlyList<(string, string)> TTranscriptionRowsRead(CTranscriptionSheet sheet)`

The scheme and text of every row the block answers, in its order.

## `private static CTranscription TTranscriptionEmptyPrepare()`

The transcription gates of an editor over stub ports, whose desk holds nothing.

## `private static CEditor TTranscriptionPrepare(LEngine engine, long entry)`

Opens the stored entry `entry` in an editor over the library vista.

## `private static long TTranscriptionSave(`

Stores a headword in `language`, Cantonese by default, with the transcription rows `rows`.
