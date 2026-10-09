# TEngineInflectionRegular.cs
Hash: `ef0b72f9970db67a`

## `public sealed class TEngineInflectionRegular`

Covers the analysis the engine stores when a form is written, and its renewal when the rule book changes.
Each fact saves one form through the entry save path, with the morphology fetch off.

## `public void ParadigmUpdate_SpanishRegularForm_SetsRegular()`

An exactly predicted Spanish form stores its prediction, empty marks, and book stamp.
Its regular flag is true.

## `public void ParadigmUpdate_TenerPreterite_ClearsRegular()`

The irregular tuve departs from the predicted teni in two letters.
So it is stored with those marks and is not regular.

## `public void ParadigmUpdate_FrenchPlural_UsesParadigmRule()`

French has no rule book, so the paradigm's own rule still sets the flag.
The three analysis columns stay `NULL`.

## `public void InflectionAnalysisSave_StoredAnalysis_ReadsBack()`

The archive stores a prediction, marks and a stamp and reads them back unchanged.
An empty mark list reads back empty, and `null` values read back as uncovered.

## `public void InflectionStart_ChangedStamp_ReanalysesWithoutFetch()`

A stored row whose stamp differs from the book's is analysed again when the background check starts.
The new analysis comes from the stored text, and no request reaches the web.

## `public void InflectionStart_NullStampCovered_AnalysesWithoutFetch()`

A stored row with no stamp, as written before the book or imported, is stale.
The background check analyses it from the stored text, and no request reaches the web.

## `public void InflectionStart_UncoveredRow_SkipsSecondAnalysis()`

A form whose headword fits no kind of the book is stored uncovered, with the book's stamp and no prediction.
With its stamp cleared, the first background check analyses it and the paradigm's own rules set its flag.
The second check finds the book's stamp and leaves the row alone, so a flag set by hand survives.

## `private static string TInflectionStampRead()`

The stamp of the shipped Spanish book, which every fresh analysis of a Spanish row carries.

## `private static LInflection TInflectionFormSave(TWorkspace workspace, LEngine engine, string headword, string language, string speech, string key, string text)`

Saves an entry of one part of speech and one form in the slot named by `key`.
It answers the single stored row as the archive reads it.
