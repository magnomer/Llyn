# LScriptLoader.cs
Hash: `9059c851cfb800eb`

## `internal static class LScriptLoader`

The script side of the pack loader.
It reads the character styles a Han language pack lists.
Each row becomes an [LScriptStyle](../../../Llyn.Core/Pronunciation/Script/LScriptStyle.comment.md), in written order.
`LLanguageLoader` calls it.

## `private const string LScriptKey = "script";`

The key of the style list.

## `private const string LScriptEpochKey = "epoch";`

The key of a chronology table, in the pack and in a style row.

## `public static IReadOnlyList<LScriptStyle> LScriptPackScan(JsonElement root)`

Reads every row of the `script` array, skipping rows the style reader refuses.
A pack without the array yields an empty list, and the reading view shows no script box.
The pack's own `epoch` table is read once here and handed to every row.
One source dates its bronzes, seals and clerical hands alike, so one table serves them all.

## `private static LScriptStyle? LScriptRowRead(JsonElement row, IReadOnlyList<LEpoch> shared)`

One style row: its name, the address the form is posted to, and the pattern reading the answer.
A row missing any of the three is dropped, because none of them has a default worth guessing.
The group numbers, prefix, rewrite rules, gloss pattern and chronology labels are optional.
A row listing its own labels keeps them whole, and any other row takes the pack's.
A style is a page of one site, so it overrides the table only where that page dates differently.

## `private static IReadOnlyList<LEpoch> LScriptEpochScan(JsonElement row)`

The `epoch` array as ordered [LEpoch](../../../Llyn.Core/Pronunciation/Script/LEpoch.comment.md) pairs, each a label and its code.
The same reader serves the pack and a style row, since both write the table the same way.
A pair missing either half is skipped.
A label with no code dates nothing, and a code with no label is never met.
A pack whose captions carry no age lists none, and its captions are stored whole.
