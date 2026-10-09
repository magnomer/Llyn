# TLacunaCell.cs
Hash: `7987b0edee4d5f16`

## `public sealed class TLacunaCell`

Covers the cell column of the lacuna store.

## `public void LacunaSave_CellMiss_ReadsBackCell()`

A one-value miss and a cell miss on the same first value are both kept.
Each reads back with its own cell, the one-value row with an empty one.

## `public void DatabaseCreate_OlderLacuna_CarriesRowsWithEmptyCell()`

A workspace one version older has a lacuna table without the cell column.
The rebuild carries its row across, and the row takes the empty cell.
