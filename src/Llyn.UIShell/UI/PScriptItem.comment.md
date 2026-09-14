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

## `internal static IReadOnlyList<PScriptItem> PScriptItemScan(IReadOnlyList<LScriptImage> images, IReadOnlyList<LScriptStyle> styles)`

Groups the pictures by character in headword order, then by style in pack order.
A style with no picture for a character has no row.

## `private static PScriptItem? PScriptItemCreate(string character, string style, IReadOnlyList<LScriptImage> group)`

Decodes one group's pictures and takes the first gloss it carries.
A group whose every picture fails to decode makes no row.
