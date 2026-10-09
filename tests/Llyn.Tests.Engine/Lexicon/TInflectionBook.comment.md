# TInflectionBook.cs
Hash: `c7bde04a10d9255d`

## `public sealed class TInflectionBook`

Covers the inflection rule engine on a book built in code from the design's Spanish side file.
The engine itself names no language, so the test hands it the Spanish data as the pack file does.
Mark assertions compare predictions with actual forms through the book's folds, mirroring stored-analysis computation.

## `public void InflectionBookScan_RegularRow_MarksNothing(string headword, string values, string predicted, string actual)`

Each row is one tense of recibir or hablar across its six person columns.
Every prediction matches the design table, and every regular form gets no marks.
A dash column has no ending, so that cell is uncovered.

## `public void InflectionBookScan_HandCheck_MarksStatedLetters(string headword, string cell, string predicted, string actual, string marks)`

The hand checks beyond the two model verbs, with their marks written as stored text.
Tener marks its irregular stem, construir its inserted y, and seguir its changed vowel.
Spelling alternations such as c to qu, z to c and j to g fold away unmarked.

## `public void InflectionBookScan_NoKind_ReturnsNull()`

A headword no kind matches leaves every cell uncovered.

## `public void InflectionBookScan_NoEnding_ReturnsNull()`

The first singular imperative has a null ending, so it is uncovered.
A partial cell is uncovered, while an out-of-order complete imperative cell remains covered.

## `public void InflectionBookResolve_KindLimitedRule_SkipsOtherKinds()`

The vowel raising rule names the ir kind, so comer keeps its e and vivir raises it.

## `public void InflectionBookDivide_RootMarker_AnswersTextAndRoot()`

Each design template marks its root, so the root survives the final marker rule.
Hablo, tenes and teni keep their roots after the stem vowel is rewritten or kept.

## `public void InflectionBookDivide_NoRootMarker_AnswersNullRoot()`

A template without the marker still predicts its text.
The root is null, so a view keeps the stored marks for such a pack.

## `internal static IReadOnlyList<LInflectionMark>? TInflectionBookScan(LInflectionBook book, string headword, IReadOnlyList<long> codes, string actual)`

Predicts the cell once and compares `actual` with the prediction through the book's folds.
Null means the cell is uncovered, and an empty list means the form is regular.
The loader facts share it, so every mark assertion reads the same two steps.

## `internal static LInflectionBook TInflectionBookCreate()`

Builds the design's book, zipping each stem's endings with the six columns and skipping null endings.
Prediction tests omit layout because prediction and difference scanning do not consume it.
The non-ASCII kind identifier verifies that class names remain opaque keys.
