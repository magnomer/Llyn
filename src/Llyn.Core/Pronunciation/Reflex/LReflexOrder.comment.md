# LReflexOrder.cs
Hash: `517b53769a2f330d`

## `public sealed record LReflexOrder(IReadOnlyList<string> LReflexOrderLanguages, IReadOnlyDictionary<string, IReadOnlyList<string>> LReflexOrderKinds)`

The order a pack declares for the reflex rows of its entries.
It comes from the pack's `order` key, apart from the fetch rules.
So the order of the fetch rules never reaches the screen.

**Parameters**

- `LReflexOrderLanguages` — The borrowing languages in the order their rows are listed.
- `LReflexOrderKinds` — Per borrowing language, the kinds in the order its rows are listed.

## `public static LReflexOrder LReflexOrderEmpty { get; }`

The order of a pack that declares none.
Its rows then list by language name and kind name.

## `public IReadOnlyList<LReflexDraft> LReflexOrderSort(IReadOnlyList<LReflexDraft> rows)`

The one rule ordering the reflex rows of an entry for every view.
Declared languages come first in their declared order, and the rest follow by name.
Within a language, declared kinds come first in their order, and the rest follow by name.
Within a language and kind, the main row comes first, and the rest follow by reading text.
The stored id is only the last tie-break, so the result is stable.
Storage order never decides, since a fetch stores rows in whatever order its rules run.

## `internal static int LReflexOrderFind(IReadOnlyList<string> declared, string name)`

The place of a name in a declared list.
A name the list lacks sorts after every declared one.
The match is exact, since row names are written from the same pack that declares the list.
`LTallyLine.LTallyLineScan` ranks its lines with it too, so both lists order a pack alike.
