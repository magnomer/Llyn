# CParadigmSlot.cs

## `public sealed record CParadigmSlot(`

One row of a paradigm, as the paradigm table lists it.
The controller joins the engine's slots into rows, so the driver never groups them.
Each row arrives with the text to show and the key of its tip, so the driver decides neither.

**Parameters**

- `CParadigmSlotPart`: the part of speech when the row leads its part, else empty.
- `CParadigmSlotName`: the joined morphology names of the row.
- `CParadigmSlotText`: the inflected form, or the mark that stands in for a missing one.
- `CParadigmSlotTip`: the localization key of the tip, or null when the row shows its form.
