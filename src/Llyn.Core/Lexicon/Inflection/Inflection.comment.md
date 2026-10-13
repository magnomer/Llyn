# Inflection

Inflection holds a language pack's rule book, which predicts regular forms and marks irregular letters.
Conjugation templates and rewrites stay language-pack data rather than engine branches.
It reads strings and codes only, with no storage behind it.

## `LInflectionBook`

The rule book of one pack, with kinds, stems, rules, folds, layout and a stamp.
`LInflectionBookDivide` predicts the regular form of a headword for a cell given in any order.
It also gives where the predicted root ends, so a view can split root from ending.
The root boundary is the `·` a pack template writes after the root.
While a rule leaves the `·` in place, the boundary follows its new index.
A rule that drops it maps the boundary through that rule's matches in order.
A boundary inside a match lands where the match's replacement starts.
A boundary after a match shifts by the length change of the matches before it.
The root is null when the applied template writes no `·`, or when prediction returns null.
The answered root is clamped to the predicted text.
`LInflectionBookResolve` is the same text with the root discarded, so prediction keeps one pipeline.
Cell matching compares sorted value sequences, keeping duplicate counts rather than treating codes as a set.
The first matching stem-ending pair wins, even if its missing template makes prediction return null.
The template comes from the first matching kind, in book order, that the stem names.
Null means no kind matches, no cell matches, or no matching kind supplies that stem's template.
Kind-restricted rewrites consider every matching headword kind, not only the template's kind.

## `LInflectionDifference`

Compares a predicted form with the actual one and marks what the prediction missed.
Both sides undergo invariant lowercasing and ordered folds before comparison.
`LInflectionDifferenceScan` marks actual UTF-16 spans outside a longest common subsequence of the folded forms.
Tie-breaking skips predicted units first, which fixes which unmatched actual spans are selected.
Missing predicted units alone produce no mark because no actual span represents them.
A replacement unit carries its entire original match span, including both units of a folded digraph.
Overlapping and touching ranges merge, but separated differences remain separate.
`LInflectionDifferenceDivide` finds where the ending of the actual form starts, given the predicted root end.
It walks the same alignment as the scan, so marks and split agree.
The answer is the actual offset of the first letter matched to a predicted letter at or past the root.
Inserted actual letters before that match therefore belong to the root.
With no such match the whole form is root.
Both walks build their table in `LInflectionDifferenceCreate`, so they cannot drift apart.
`LInflectionDifferenceResolve` keeps one mapping per original UTF-16 unit before ordered folds.
Zero-width replacements inherit empty spans and cannot create visible marks.
Fold kind restrictions are not consulted, because this comparison has no headword classification.

## `LInflectionEnding`

The ending of one column in a stem's row.
The text may carry operator characters that only the book's rules give meaning.

## `LInflectionKind`

One headword class of a rule book, named by the pack and recognised by a pattern.
A headword may belong to several kinds at once.
Named groups in the pattern feed the stem templates, so `${root}` reads the group `root`.
Matching ignores case and culture and waits at most one second.
Equality compares the two strings, because the compiled pattern differs per instance.
A timeout in `LInflectionKindCheck` excludes this kind without deciding whether another kind can predict the cell.
`LInflectionKindResolve` leaves unmatched text around the first replacement, so whole-headword templates need anchored patterns.
A match that times out returns the headword unchanged.

## `LInflectionLetter`

One folded UTF-16 unit that retains its original text span, empty for zero-width replacements.
`LInflectionDifference` compares these and marks the spans of the unmatched ones.

## `LInflectionMark`

One range of the actual form that the rules did not predict.
Offsets and lengths count UTF-16 code units of the stored form, not its folded copy.
`LInflectionMarkFormat` writes `offset:length` pairs joined by commas, in invariant culture.
No marks give the empty string, which differs from a missing analysis.
`LInflectionMarkParse` skips malformed, signed, whitespace-padded or overflowing pairs independently.
Zero-length ranges are accepted, and parsing does not validate bounds against form text.

## `LInflectionRule`

One ordered rewrite of a rule book, used both for spelling rules and for comparison folds.
Rules run in book order, and each one replaces every match.
Prediction checks kind restrictions, but this record's rewrite and scan methods do not.
The regex is culture-invariant and case-sensitive by default, with a one-second timeout.
Equality compares the three strings, because the compiled pattern differs per instance.
A match that times out in `LInflectionRuleResolve` leaves the text unchanged, so one slow rule never fails the prediction.
`LInflectionRuleScan` serves a caller that tracks where each replacement came from.
A timeout discards the entire scan, letting the fold caller preserve this rule's input.

## `LInflectionStem`

One paradigm row combining kind-specific stem templates with the endings of its covered columns.
The row's values plus one ending's values make the full cell the stem answers.
Prediction can use another matching kind's template when one matching kind lacks a template.

## Stamp

Commit: `c2f335c331fe`
- `LInflectionBook.cs` `4b011890a5e05e31`
- `LInflectionDifference.cs` `2a1645adf8da5f9b`
- `LInflectionEnding.cs` `1508c10daab8aea5`
- `LInflectionKind.cs` `03e0f84167e2d61a`
- `LInflectionLetter.cs` `767bc789323b28d2`
- `LInflectionMark.cs` `5122222b0e97fbcf`
- `LInflectionRule.cs` `62c7a8c70cf48ea5`
- `LInflectionStem.cs` `bc8d0e33bd7da549`
