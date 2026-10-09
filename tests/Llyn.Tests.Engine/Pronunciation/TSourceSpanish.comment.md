# TSourceSpanish.cs
Hash: `19854b0c39cfbb71`

## `public sealed class TSourceSpanish`

Covers the shipped Spanish `morphology` source reading one recibir page.
The readings come from the pack itself, so the test reads the patterns the app fetches with.
Each reading must hit exactly one cell of the Spanish table and nothing in the decoy.

## `public async Task SourceFind_SpanishTable_ReturnsSixtyFourCellsKeyedByCode()`

The page answers 64 readings, one per verb cell, in the paradigm's cell order.
Each variety is the cell key, so the fetch can match it to its slot.
Expected forms come from the embedded recibir fixture, not a live request.

## `public async Task SourceFind_VosSecond_KeepsTheTuForm()`

A 2sg cell carrying tú and vos answers the tú form.
The vos form of the present subjunctive appears nowhere in the answer.

## `public async Task SourceFind_NegativeImperative_SkipsNo()`

Each negative imperative cell answers the form after `no`, never `no` itself.

## `public async Task SourceFind_DecoyTableFirst_ReadsSpanishOnly()`

The Galician table comes first and reuses the imperative titles.
The answer still holds the Spanish 2pl imperatives and no Galician form.

## `private static async Task<IReadOnlyList<LReading>> TSourceSpanishRead()`

Loads the Spanish pack's one morphology source and runs it on the fixture body.
The fake client answers the body for any address, so the live URL is never called.

## `private const string TSourceSpanishBody`

Embedded recibir HTML exercises language anchoring, alternate forms, empty cells, and nested header markup.
A Galician decoy holds only its two imperative rows, with the forms recibide and recibades.
The affirmative decoy retains an empty first cell, exposing incorrect second-plural reads without language anchoring.
The Spanish table keeps the eleven finite rows and drops the nonfinite rows and person headers.
The 2sg cells of the present indicative, present subjunctive and affirmative imperative carry vos second.
The imperative rows keep the empty first cell.
The future subjunctive header keeps its footnote `<sup>`.

## `private static readonly string[] TSourceSpanishForms`

Expected fixture forms follow paradigm cell order, independently checking each extracted form.
