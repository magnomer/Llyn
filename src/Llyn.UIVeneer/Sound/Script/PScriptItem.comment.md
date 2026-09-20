# PScriptItem.cs

## `public sealed class PScriptItem`

One row of the script box: one character in one style, with every picture of it.
A row is built once from the engine's pictures and never changes.

## `public string PScriptItemCharacter`

The character heading the row, or empty.
It is empty when the row is not the first of its character, or when the headword is one character.
A one-character headword already stands above the box, so the row does not repeat it.

## `public string PScriptItemStyle`

The style name from the pack, drawn as the row's chip.

## `public string PScriptItemGloss`

The gloss the source printed for the style, drawn under the pictures, or empty so the template collapses it.

## `public IReadOnlyList<PScriptImage> PScriptItemImages`

The decoded pictures of the row, in stored order.

## `internal static IReadOnlyList<PScriptItem> PScriptItemScan(IReadOnlyList<LScriptGroup> groups)`

One block per group the engine handed over, skipping a group whose pictures all fail to decode.

## `private static PScriptItem? PScriptItemCreate(LScriptGroup group)`

Decodes one group's pictures under the heading, style and gloss the engine chose.
A group whose every picture fails to decode makes no row.

