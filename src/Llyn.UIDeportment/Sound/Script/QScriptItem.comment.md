# QScriptItem.cs
Hash: `c07b2dae6b40fd7a`

## `internal sealed class QScriptItem`

One row of the script box: one character in one style, with every picture of it.
A row is built once from the engine's pictures and never changes.

## `public string QScriptItemCharacter`

The character heading the row, or empty.
It is empty when the row is not the first of its character, or when the headword is one character.
A one-character headword already stands above the box, so the row does not repeat it.

## `public string QScriptItemStyle`

The style name from the pack, drawn as the row's chip.

## `public string QScriptItemGloss`

The gloss the source printed for the style, drawn under the pictures, or empty so the template collapses it.

## `public IReadOnlyList<QScriptImage> QScriptItemImages`

The pictures of the row, decoded when first seen, in stored order.

## `internal static void QScriptItemRefine(FrameworkElement container, object item, string? _)`

Fills a row of `Theme.Script.Row` with character, style chip, gloss and pictures.
The picture list is attached to `QScriptImage.QScriptImageRefine`.

## `internal static IReadOnlyList<QScriptItem> QScriptItemScan(IReadOnlyList<CScriptGroup> groups, Action<Exception> failure)`

One row per group the engine handed over, skipping a group whose pictures all fail to decode.
The first decode failure of the scan reaches `failure`, and later ones are dropped.
So one scan shows one notice, however many pictures fail now or when they come into view.
The drop stays here, because another driver would decode with its own library.

## `private static QScriptItem? QScriptItemCreate(CScriptGroup group, Action<Exception> failure)`

Decodes one group's pictures under the heading, style and gloss the engine chose.
A group whose every picture fails to decode makes no row.
