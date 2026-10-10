# TEngineParadigmView.cs
Hash: `b0d30c06175ac036`

## `public sealed class TEngineParadigmView`

Covers the inflection view the engine answers for an entry.
Covers how the list and the font language change beside it.
Fixtures disable morphology fetching and save entries, with forms added only where needed.

## `public void InflectionRead_SpanishVerb_AnswersCollapsedAndExpanded()`

With custom analysis off, a Spanish verb answers both default views from the pack's layout.
The collapsed view has nine lines and no headers.
The expanded view has eleven lines under six header keys.
Labels and headers come through as localization keys.
A stored regular form shows unmarked text with no tip.
An existing slot without a form has the lost tip.

## `public void InflectionRead_LostCellWithHeld_AnswersHeldTip()`

The held rule lives in Core, so the engine test keeps it covered.
With held on, an existing slot without a form shows the ellipsis.
That cell carries the held tip instead of the lost tip.

## `public void InflectionRead_Tener_MarksWholeRoot()`

The irregular tuve is stored through the write path with its marks.
The stored row keeps the two marked letters.
The view answers one mark over the whole word tuve, with no split.
No letter aligns past the predicted root, so the whole word reads as the root.
The preterite line is found by its label, since custom analysis is on.

## `public void InflectionRead_UnfetchedCell_AnswersPendingStatus()`

A cell with no form during a fetch shows the ellipsis with the pending tip.
The slotless first-singular imperative instead has empty text and no tip.

## `public void InflectionRead_AnalysisOn_UsesCustomOrderAndMarks()`

Custom analysis is on by default.
Both views then follow the custom sheets, which open with the future.
In the collapsed view the irregular tuve is coloured as a whole root.

## `public void InflectionRead_AnalysisOn_SplitsRootFromEnding()`

The irregular tengo splits after teng, with the inserted g kept in the root.
The root is marked as a whole, and the ending stays unmarked.
The present line is found by group and label, since the subjunctive also has one.

## `public void InflectionRead_AnalysisOff_UsesDefaultOrderWithoutMarks()`

With custom analysis off, both views open with the indicative.
The expanded view lists the subjunctive before the imperative.
Every collapsed form and the expanded tuve show no marks, while the stored row keeps its marks.

## `public void InflectionRead_FrenchEntry_AnswersNull()`

French has no layout, so there is no view.

## `public void ParadigmScan_SpanishVerb_LeavesVerbSlotsToTheView()`

The list drops every slot of the layout's part, even an irregular one the display would otherwise show.

## `public void LanguageResolve_AllRegular_StillAnswersLanguage()`

Every cell holds the form the book predicts, so no display slot remains.
The font language still names Spanish, since it reads every slot.

## `private static LParadigmLine TParadigmLineFind(LParadigmTable table, string label)`

The single line of `table` whose label key is `label`.

## `private static LEntry TParadigmEntrySave(LEngine engine, string headword, string language, string speech)`

Saves an entry in `language` with one meaning and the part `speech`.

## `private static void TParadigmFormSave(LEngine engine, LEntry entry, string key, string text)`

Stores `text` as the entry's only form, on the slot whose cell key is `key`.
