# TParadigmCell.cs
Hash: `9448ea67ee03840b`

## `public sealed class TParadigmCell`

Covers paradigm cells, the forms a pack names by several values at once.
The loader facts write a vocabulary into an unlisted fixture language, so no other test sees it.
The slot facts build rows in memory.

## `public void SpeechPackLoad_ParadigmCells_ReadsEachCellInOrder()`

A row's values come first as one-value cells, then its stated cells in pack order.
A row may state cells alone, and it then has no one-value cell.
A values-only row retains legacy one-cell-per-value loading without requiring explicit cells.

## `public void SpeechPackLoad_CellsMalformed_SkipsRow()`

A cell holding a non-number, a non-positive code or nothing at all drops its row.
So does a cell list that is not a list of lists, even beside valid values.
The well-formed neighbour is kept.

## `public void ParadigmSlotKey_ManyValues_JoinsCodesAscending()`

The key sorts the codes, so the -ra value listed third still sorts last.

## `public void ParadigmSlotName_ManyValues_JoinsNamesInCellOrder()`

The name keeps the pack order, and the row label shows it whole.

## `public void ParadigmRowName_OneValue_KeepsTodaysName()`

A one-value slot keys on its bare code and names itself by its morphology.
Equal-text slots still combine morphology names with comma separation.

## `private static LParadigmSlot TParadigmCellCreate()`

A Spanish first singular imperfect subjunctive slot of the -ra form, its values listed out of code order.
