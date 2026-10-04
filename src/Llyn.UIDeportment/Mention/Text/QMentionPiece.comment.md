# QMentionPiece.cs
Hash: `b7c7137a742199df`

## `internal sealed record QMentionPiece(string QMentionPieceText, bool? QMentionPieceLinked)`

One run of a shown sentence, as the mention text draws it.
It carries the run's text and its link state, so the control never holds a Conduct piece.

**Parameters**

- `QMentionPieceText`: the run's text.
- `QMentionPieceLinked`: whether the run's Mention links an entry, null for plain text.

## `internal static IReadOnlyList<QMentionPiece> QMentionPieceCreate(IReadOnlyList<CMentionPiece> pieces)`

The runs of a sentence, one per ready piece, in the order Conduct divided them.
It only copies, so the driver neither cuts nor judges a span.

## `internal static IReadOnlyList<QMentionPiece> QMentionPieceCreate(string text)`

A whole text as one plain run starting at code point zero, as a text without Mentions divides.
The corpus placeholder word and the etymology prose come through here, so the mention text takes only pieces.
