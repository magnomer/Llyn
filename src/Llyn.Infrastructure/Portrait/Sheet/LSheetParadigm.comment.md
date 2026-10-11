# LSheetParadigm.cs
Hash: `210fb09dca0eec79`

## `internal static class LSheetParadigm`

Writes the paradigm box of an exported entry body, as `QParadigm` draws it in view mode.

## `internal static void LSheetParadigmAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)`

Writes one `paradigm` box without a heading, as the view shows none.
The slot rows come first, one per `LLiveryPageParadigm` row.
Each shows the part, the name and the paired `LLiveryPageShown` text.
`LLiveryPageShown` runs parallel to the rows, so both share one index.
A shown tip is a localization key, worded through `lookup` into the cell's `title`.
The inflection sheets follow, their rows from `LSheetInflection.LSheetInflectionFormat`.
Two sheets that differ get the switch, then the Short table, then the Full table.
`LSheetInflection.LSheetInflectionResolve` decides between one table and the switch.
`LLiveryInflection` asks it too, so the export and Joplin never disagree.
A page with no rows and no sheet lines writes no box.

## `private static void LSheetSwitchAppend(StringBuilder sheet, Func<string, string> lookup)`

Writes two radios, Short checked as the view starts, then their labels `Paradigm.Short` and `Paradigm.Full`.
The radios sit directly in the box, siblings of the two tables.
The labels sit in a `switch` holder titled `Paradigm.Fold`, the pill `Theme.Paradigm.Switch` draws.
So a style rule can hide the table whose radio is not checked, without script.
The ids are fixed, since an entry sheet holds one paradigm box.
