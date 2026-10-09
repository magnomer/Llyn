# TInflectionDifference.cs
Hash: `fb24deb66540ca5e`

## `public sealed class TInflectionDifference`

Covers the comparison of a predicted form with the actual one, and the stored text of marks.
The folds come from the design's book in `TInflectionBook`.

## `public void InflectionDifferenceScan_FoldedDigraph_MarksBothLetters()`

The qu of bosque folds to one c, so an unmatched c marks both original letters.

## `public void InflectionDifferenceScan_AccentOnly_MarksNothing()`

Case, accents and the dieresis fold away, so they never count as irregular.

## `public void InflectionDifferenceScan_Inserted_MarksOneRange()`

Two inserted letters touch, so they merge into one range over the accented form.

## `public void InflectionDifferenceDivide_Tener_AnswersEndingStart(string predicted, string actual, int split)`

The predicted forms of tener keep the root ten, given by hand.
The inserted g of tengo and the inserted i of tienes fall into the root.
A regular tenemos keeps the predicted split.

## `public void InflectionMarkFormat_TwoMarks_JoinsPairs()`

Marks are written as offset and length pairs, and no marks write the empty string.

## `public void InflectionMarkParse_Formatted_RoundTrips()`

Parsing the written text gives back the same marks.

## `public void InflectionMarkParse_Malformed_SkipsPairs()`

Non-numbers, negatives, missing or extra parts and empty pieces are skipped, and good pairs survive.
