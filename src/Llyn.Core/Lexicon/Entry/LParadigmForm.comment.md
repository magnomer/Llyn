# LParadigmForm.cs
Hash: `b30f13bfad4d5f4d`

## `public sealed record LParadigmForm(string LParadigmFormText, IReadOnlyList<LInflectionMark> LParadigmFormMarks, string? LParadigmFormTip, int LParadigmFormSplit = 0)`

One cell of an inflection view, ready to show.
A cell without a written form takes its text and tip from `LParadigmShown.LParadigmShownResolve`.
So no reader applies the status rule.
The text is the stored form when the cell shows one, else the mark that stands in for it.
The marks come from the analysis stored with the form, widened to whole parts when divided.
Only the split asks the rule book again, for the predicted root.
A cell the entry has no slot for is empty text with no marks.

**Parameters**

- `LParadigmFormText`: the stored form, the stand-in mark of a missing one, or empty for a blank cell.
- `LParadigmFormMarks`: the stored irregular ranges, empty when none, uncovered or outside the custom view.
- `LParadigmFormTip`: the localization key of the tip, or null when the cell shows its form or is blank.
- `LParadigmFormSplit`: the offset where the ending starts, or 0 when the form is not divided.
  A divided form always has letters on both sides of its split.

## `public IReadOnlyList<LParadigmPiece> LParadigmFormPiece { get; init; }`

The pieces a reader paints, empty until the paradigm clerk fills them from `LParadigmFormScan`.
Core's view build leaves them empty, so only Application calls the walk for the app.
Conduct maps them as data and never calls Core.

## `public IReadOnlyList<LParadigmPiece> LParadigmFormScan()`

Cuts the text into the pieces a reader paints, the only copy of that walk.
A new piece starts wherever the marking changes and at the split.
Marks are clamped to the text, so a stale range never throws.
Only the piece ending at a split inside the text carries the cut.
Empty text answers no piece.
The app box and the sheet writer `LSheetInflection` both paint these pieces, so their cells agree.
