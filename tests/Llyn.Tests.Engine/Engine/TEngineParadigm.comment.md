# TEngineParadigm.cs
Hash: `16fddf3b493beeab`
Hash: `bd8be2a5bb881dbc`

## `public sealed class TEngineParadigm`

Covers the engine's answer to which forms an entry is expected to have.
Most tests save an entry through the engine against the shipped English pack.
So the paradigms and the morphology rows are the real ones and not a fixture.

## `public void ParadigmShow_DeclaredPart_ReturnsItsSlots()`

A part declaring a paradigm shows its slots in the declared order, each carrying that part.
With nothing stored every slot is unspecified.

## `public void ParadigmShow_StoredInflection_MarksSlotSpecified()`

An appended inflection carrying a slot's morphology row answers that slot and makes it specified.
Its neighbour stays unspecified.

## `public void ParadigmShow_RetiredSubtypeName_ReturnsNothing()`

A retired subtype typed by name is a custom part, so it shows no slots.
A noun shows its one.
The packs list bare roles only, so no paradigm reaches a name the pack no longer declares.

## `public void ParadigmShow_CustomPart_ReturnsNothing()`

A part typed by hand declares no paradigm, and an entry the workspace does not hold answers nothing either.

## `public void ParadigmShow_ManyParts_ReturnsSlotsInPartOrder()`

An entry with several parts lists each part's slots in the order the parts are stored.

## `public void MorphologyCodeFind_PackCode_ReturnsLanguageRow()`

A pack code resolves to the morphology row of its language.
A code or language the workspace lacks answers null.

## `public void ParadigmMatch_NounPattern_AcceptsRegularForm()`

The shipped noun pattern accepts a regular plural regardless of case and rejects an irregular one.
A paradigm stating no pattern answers false.

## `public void ParadigmShow_SharedForm_FillsBothSlots()`

One form that carries both morphology rows of a verb fills both slots.
The two slots then answer the same stored inflection, so the form is held once and not twice.

## `public void ParadigmShow_RegularNounPlural_DropsSlot()`

A stored plural the shipped noun pattern accepts is dropped from the display rows while the read still lists it.

## `public void ParadigmShow_IrregularNounPlural_KeepsSlot()`

A stored plural the pattern rejects stays a display row, still specified.

## `public void ParadigmShow_VerbWithoutPattern_KeepsBothSlots()`

The shipped English verb slots keep both stored forms while declaring no paradigm regularity rules.

## `public void ParadigmShow_UnfilledSlot_KeepsSlot()`

An unfilled slot is never regular, so it shows, and an entry the workspace lacks answers nothing.

## `public async Task MorphologySave_Off_CancelsPendingFetch()`

Turning the morphology switch off while a fetch waits on the web cancels it.
The forms the source later returns are discarded and the slots stay unspecified.

## `private const string TParadigmFetchBody`

A source page carrying both verb forms, held behind a gate so the switch can flip mid-flight.

## `private static void TParadigmInflectionAppend(LEngine engine, long entryId, params string[] forms)`

Stores one form per slot, in slot order, so a test fills a paradigm in one line.

## `private static void TParadigmInflectionSave(LEngine engine, long entryId, IReadOnlyList<LInflection> added)`

Adds forms after the stored ones through the entry update, which regrades every form it stores.

## `private static LEntryDraft TParadigmDraftCreate(string headword, params string[] speeches)`

Builds a minimal English draft for `headword` carrying the given parts of speech by name.
