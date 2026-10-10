# CParadigmForm.cs
Hash: `8f54fcdf81586f39`

## `public sealed record CParadigmForm(string CParadigmFormText, IReadOnlyList<CParadigmMark> CParadigmFormMarks, string? CParadigmFormTip, int CParadigmFormSplit = 0)`

One cell of the inflection box, ready to show.
The text is the form or the mark standing in for a missing one.

**Parameters**

- `CParadigmFormText`: the inflected form, or the mark that stands in for a missing one.
- `CParadigmFormMarks`: the ranges to paint, empty unless the form is shown.
- `CParadigmFormTip`: the localization key of the tip, or null when the cell shows its form.
- `CParadigmFormSplit`: where the ending starts, or 0 when the form is not divided.

## `internal static CParadigmForm CParadigmFormCreate(LParadigmForm form)`

Maps one ready cell of the box, its text and tip key as the engine answered them.
The wording stays in Core, so the box, the plain slot list and the Joplin note agree.
It copies the marked ranges and the split as Core gave them, whatever the status.
Core leaves both empty for a cell that shows a status, so no check is needed here.
The ranges are copied into an array, so an empty source keeps the shared empty value.
