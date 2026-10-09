# CParadigmForm.cs
Hash: `3715e9d57837238e`

## `public sealed record CParadigmForm(string CParadigmFormText, IReadOnlyList<CParadigmMark> CParadigmFormMarks, string? CParadigmFormTip, int CParadigmFormSplit = 0)`

One cell of the inflection box, ready to show.
The text is the form or the mark standing in for a missing one.

**Parameters**

- `CParadigmFormText`: the inflected form, or the mark that stands in for a missing one.
- `CParadigmFormMarks`: the ranges to paint, empty unless the form is shown.
- `CParadigmFormTip`: the localization key of the tip, or null when the cell shows its form.
- `CParadigmFormSplit`: where the ending starts, or 0 when the form is not divided.

## `internal static CParadigmForm CParadigmFormCreate(LParadigmForm form, bool held)`

Maps one cell of the box through `CParadigmFormResolve`.
A shown form also carries its marked ranges and its split, copied as Core gave them.

## `internal static CParadigmForm CParadigmFormResolve(LParadigmStatus status, string text, bool held)`

Maps a status to the text shown and the key of its tip, with no marks.
It is the one owner of this wording, for the box and for the plain slot list.
A shown form keeps `text`, and every other status shows a stand-in mark.
Only lost cells use `held` to choose between held and lost tooltip keys.
An unrecognized enum value throws rather than silently displaying a placeholder.
