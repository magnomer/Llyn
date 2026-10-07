# LReflexClerkRow.cs
Hash: `e5aa7a54543a2d92`

## `public static class LReflexClerkRow`

The pure row helpers of the reflex clerk: merging fetched rounds, reading drafts as rows, matching and formatting.

## `public static IReadOnlyList<LReflexDraft> LReflexRoundResolve(IReadOnlyList<IReadOnlyList<LReflexDraft>> rounds)`

Rows from later rounds that match an earlier row by language, region, kind and romanization join its text.
A row already joined in the same round stays separate.

## `public static IReadOnlyList<LReflex> LReflexRowRead(long entryId, IReadOnlyList<LReflexDraft> drafts)`

The rows a draft list means, trimmed, each with its place in the draft as its position.

## `public static bool LReflexRowMatch(IReadOnlyList<LReflex> stored, IReadOnlyList<LReflex> current)`

Whether two row lists say the same thing, anchors compared as sets.
Each current row is paired with the stored row of its id, so position and list order are ignored.
Every view orders the rows on read, so a stored position is never compared.
A current row without a stored id, or naming one twice, is a change.

## `public static IReadOnlyList<LReflex> LReflexAnatomyClear(IReadOnlyList<LReflex> rows)`

The rows with their anatomy emptied, so a comparison ignores it.

## `public static string LReflexRowFormat(IReadOnlyList<LReflex> reflexes)`

The rows as one summary line for the revision.

## `private static IEnumerable<string> LReflexPartScan(LReflex reflex)`

The non-empty language, region, kind, reading, romanization, meaning and note of one row in display order.
