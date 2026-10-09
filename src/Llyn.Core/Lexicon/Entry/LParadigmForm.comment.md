# LParadigmForm.cs
Hash: `598ca13e625150a0`

## `public sealed record LParadigmForm(string LParadigmFormText, IReadOnlyList<LInflectionMark> LParadigmFormMarks, LParadigmStatus LParadigmFormStatus, int LParadigmFormSplit = 0)`

One cell of an inflection view, ready to show.
The text is the stored form only when the status is text, and empty otherwise.
The marks come from the analysis stored with the form, widened to whole parts when divided.
Only the split asks the rule book again, for the predicted root.
A cell the entry has no slot for is empty text with no marks.

**Parameters**

- `LParadigmFormText`: the stored form, or empty when the cell shows a status instead.
- `LParadigmFormMarks`: the stored irregular ranges, empty when none, uncovered or outside the custom view.
- `LParadigmFormStatus`: the slot status, or text for a blank cell without a matching slot.
- `LParadigmFormSplit`: the offset where the ending starts, or 0 when the form is not divided.
  A divided form always has letters on both sides of its split.
