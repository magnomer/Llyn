# CParadigmSlot.cs

## `public sealed record CParadigmSlot(`

One row of a paradigm, as the paradigm table lists it.
The controller joins the engine's slots into rows, so the driver never groups them.

**Parameters**

- `CParadigmSlotPart`: the part of speech when the row leads its part, else empty.
- `CParadigmSlotName`: the joined morphology names of the row.
- `CParadigmSlotText`: the inflected form, or null when none is written.
- `CParadigmSlotUncertain`: whether the form is marked unknown.
