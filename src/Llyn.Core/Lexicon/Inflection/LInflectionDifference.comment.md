# LInflectionDifference.cs
Hash: `2a1645adf8da5f9b`

## `public static class LInflectionDifference`

Compares a predicted form with the actual one and marks what the prediction missed.
Both sides undergo invariant lowercasing and ordered folds before comparison.

## `public static IReadOnlyList<LInflectionMark> LInflectionDifferenceScan(IReadOnlyList<LInflectionRule> folds, string predicted, string actual)`

Marks actual UTF-16 spans outside a longest common subsequence of the folded forms.
Tie-breaking skips predicted units first, fixing which unmatched actual spans are selected.
Missing predicted units alone produce no mark because no actual span represents them.
A replacement unit carries its entire original match span, including both units of a folded digraph.
Overlapping and touching ranges merge, but separated differences remain separate.

## `public static int LInflectionDifferenceDivide(IReadOnlyList<LInflectionRule> folds, string predicted, int root, string actual)`

Finds where the ending of `actual` starts, given the predicted root end `root`.
It folds both texts and walks the alignment of `LInflectionDifferenceScan`, so marks and split agree.
The answer is the actual offset of the first letter matched to a predicted letter at or past `root`.
Inserted actual letters before that match therefore belong to the root.
With no such match the whole form is root, and it answers the length of `actual`.

## `private static int[,] LInflectionDifferenceCreate(IReadOnlyList<LInflectionLetter> left, IReadOnlyList<LInflectionLetter> right)`

Builds the longest common subsequence lengths of two folded letter lists.
Each entry holds the common length of the suffixes starting at that pair of indices.
Both public walks build their table here, so they cannot drift apart.

## `private static IReadOnlyList<LInflectionLetter> LInflectionDifferenceResolve(IReadOnlyList<LInflectionRule> folds, string text)`

Invariant lowercasing preserves one mapping per original UTF-16 code unit before ordered folds.
Every replacement unit inherits the whole matched span rather than only its corresponding capture.
Zero-width replacements inherit empty spans and cannot create visible marks.
Fold kind restrictions are not consulted because this comparison has no headword classification.
