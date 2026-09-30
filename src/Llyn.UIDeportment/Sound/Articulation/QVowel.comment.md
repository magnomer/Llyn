# QVowel.cs

## `internal sealed partial class QArticulation`

The vowel chart of the articulation aid.
Rows are tongue height and columns are tongue backness, as the IPA chart arranges them.
A cell holds the unrounded vowel and then the rounded one.
The chart itself is Conduct's ready `CArticulation`, so the aid only lays it out.

## `private void QVowelIntroduce()`

Builds the chart's controls once, from the chart `CCatalogVowelRead` answers.
