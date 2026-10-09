# QParadigmLine.cs
Hash: `75464daa85fdc979`

## `public sealed class QParadigmLine`

One line of an inflection table, with its group key, its label key and its forms.

## `public string QParadigmLineGroup`

The localization key of the group heading, or empty when the line opens no group.

## `public string QParadigmLineLabel`

The localization key of the line's label, or empty for none.

## `public IReadOnlyList<QParadigmForm> QParadigmLineForms`

The forms of the line, one per column.

## `internal static QParadigmLine QParadigmLineCreate(CParadigmLine line)`

Copies one ready Conduct line, mapping each form through `QParadigmForm.QParadigmFormCreate`.
