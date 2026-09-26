# TEngineParadigm.cs

## `public sealed class TEngineParadigm`

Covers the engine's answer to which forms an entry is expected to have.
Every test saves an entry through the engine against the shipped English pack.
So the parent chain, the paradigms, and the morphology rows are the real ones and not a fixture.

## `public void ParadigmShow_ChildOfDeclaredPart_ReturnsParentSlots()`

A part naming a parent inherits the parent's paradigm, and the slots carry the child part, not the parent.
With nothing stored every slot is unspecified.

## `public void ParadigmShow_StoredInflection_MarksSlotSpecified()`

An appended inflection carrying a slot's morphology row answers that slot and makes it specified.
Its neighbour stays unspecified.

## `public void ParadigmShow_CustomPart_ReturnsNothing()`

A part typed by hand declares no paradigm, and an entry the workspace does not hold answers nothing either.

## `public void ParadigmShow_ManyParts_ReturnsSlotsInPartOrder()`

An entry with several parts lists each part's slots in the order the parts are stored.

## `public void MorphologyCodeFind_PackCode_ReturnsLanguageRow()`

A pack code resolves to the morphology row of its language.
A code or language the workspace lacks answers null.

## `public void ParadigmMatch_NounPattern_AcceptsRegularForm()`

The shipped noun pattern accepts a regular plural regardless of case and rejects an irregular one.
A paradigm stating no pattern, or one that does not parse, answers false.

## `public void ParadigmShow_RegularNounPlural_DropsSlot()`

A stored plural the shipped noun pattern accepts is dropped from the display rows while the read still lists it.

## `public void ParadigmShow_IrregularNounPlural_KeepsSlot()`

A stored plural the pattern rejects stays a display row, still specified.

## `public void ParadigmShow_VerbWithoutPattern_KeepsBothSlots()`

A paradigm stating no pattern keeps every filled slot, so both verb rows show.

## `public void ParadigmShow_UnfilledSlot_KeepsSlot()`

An unfilled slot is never regular, so it shows, and an entry the workspace lacks answers nothing.

## `public async Task MorphologySave_Off_CancelsPendingFetch()`

Turning the morphology switch off while a fetch waits on the web cancels it.
The forms the source later returns are discarded and the slots stay unspecified.

## Inline notes

### `private const string TParadigmFetchBody =`

A source page carrying both verb forms, held behind a gate so the switch can flip mid-flight.

### `private static void TParadigmInflectionAppend(LEngine engine, long entryId, params string[] forms)`

Stores one form per slot, in slot order, so a test fills a paradigm in one line.

### `private static void TParadigmInflectionSave(LEngine engine, long entryId, IReadOnlyList<LInflection> added)`

Adds forms after the stored ones through the entry update, which regrades every form it stores.

### `private static LEntryDraft TParadigmDraftCreate(string headword, params string[] speeches)`

Builds a minimal English draft for `headword` carrying the given parts of speech by name.
