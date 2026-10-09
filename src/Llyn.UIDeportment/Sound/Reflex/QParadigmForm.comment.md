# QParadigmForm.cs
Hash: `bffc2b99d401eb97`

## `public sealed class QParadigmForm`

One cell of the inflection box, ready to draw.
Conduct already chose the text, the marks and the tip key.

## `public string QParadigmFormText`

The inflected form, or the mark that stands in for a missing one.

## `public IReadOnlyList<QParadigmMark> QParadigmFormMarks`

The ranges of the text to paint, empty unless the form is shown.

## `public string? QParadigmFormTip`

The localization key of the tip, or null when the cell shows its form.

## `public int QParadigmFormSplit`

Where the ending of the text starts, or 0 when the form is not divided.

## `internal static QParadigmForm QParadigmFormCreate(CParadigmForm form)`

Copies one ready Conduct cell, mapping each mark through `QParadigmMark.QParadigmMarkCreate`.
The split is copied as given.
