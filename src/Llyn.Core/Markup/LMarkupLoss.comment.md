# LMarkupLoss.cs

## `public sealed record LMarkupLoss(`

One link an export could not write, because its target stands outside the exported set.

A key names a row only inside the file that declares it.
A relation, a translation or a synonym pointing at an entry the file does not carry has no key to write.
The format could grow a way to name an entry by headword and language instead.
It does not, because a headword is not an identifier and two entries may share one.
Resolving by headword on import would attach the link to the wrong row, which is worse than losing it.

So such a link is dropped on export, and this record says which one was dropped.
The loss is returned from the engine seam rather than written into the file.
Dropping it in silence is the one thing the format does not do.

**Parameters**

- `LMarkupLossEntry` — The headword of the entry whose card held the link.
- `LMarkupLossKind` — Which link it was: `relation`, `translation` or `synonym`.
- `LMarkupLossTarget` — The row the link pointed at, named as the workspace names it.

## Inline notes

### `public static IReadOnlyList<LMarkupLoss> LMarkupLossRead(LMarkup.LMarkupDocument document, IReadOnlyDictionary<string, string> keys)`

Every link of the document whose target the document declares no key for.

The keys map every row the file will name to the key it is named under.
A target absent from that map is a target the file cannot write.
The walk covers sub-senses, because a sub-sense links exactly as its parent does.
Order follows the document, so a report reads in the order the file was written.

**Parameters**

- `document` — The entries about to be written.
- `keys` — Every stored row id, and the document key it is written under.

### `private static string LMarkupLossRead(string entry, string meaning)`

The one row a target names, whichever of the two fields carries it.
A target names an entry or a sense and never both, so one field is always empty.
