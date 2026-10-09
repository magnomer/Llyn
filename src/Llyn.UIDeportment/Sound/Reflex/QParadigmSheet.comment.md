# QParadigmSheet.cs
Hash: `d8b4e9a8721103fa`

## `public sealed class QParadigmSheet`

The inflection view of one entry as the paradigm box draws it.
It holds the collapsed table and the expanded one.
The driver maps it from the Conduct view, so the box never reads a Conduct record.
A sheet is built once from one view and never changes.

## `public QParadigmTable QParadigmSheetCollapsed`

The short table, shown while the switch is unchecked.

## `public QParadigmTable QParadigmSheetExpanded`

The full table, shown while the switch is checked.

## `internal static QParadigmSheet? QParadigmSheetCreate(CParadigmView? view)`

Copies a ready Conduct view into a sheet, or answers `null` for no view.
The lectern and the editor both map through it.
