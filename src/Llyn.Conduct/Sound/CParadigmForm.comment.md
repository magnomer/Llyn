# CParadigmForm.cs
Hash: `fd48d5b7ad4fcb36`

## `public sealed record CParadigmForm(IReadOnlyList<CParadigmPiece> CParadigmFormPiece, string? CParadigmFormTip)`

One cell of the inflection box, ready to show.
Its pieces join to the form or the mark standing in for a missing one.

**Parameters**

- `CParadigmFormPiece`: the runs of the form or mark to paint, in order, cut before the engine answers.
- `CParadigmFormTip`: the localization key of the tip, or null when the cell shows its form.

## `internal static CParadigmForm CParadigmFormCreate(LParadigmForm form)`

Maps one ready cell of the box, its pieces and tip key as the engine answered them.
The wording stays in Core, so the box, the plain slot list and the Joplin note agree.
It maps each piece the engine answered in `LParadigmForm.LParadigmFormPiece`, whatever the status.
It calls no Core method, since the paradigm clerk already cut the pieces.
Core leaves marks and split empty for a cell that shows a status, so its one piece is plain.
No check is needed here.
