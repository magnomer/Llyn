# LInflectionBook.cs
Hash: `4b011890a5e05e31`

## `public sealed record LInflectionBook(IReadOnlyList<LInflectionKind> LInflectionBookKinds, IReadOnlyList<LInflectionStem> LInflectionBookStems, IReadOnlyList<LInflectionRule> LInflectionBookRules, IReadOnlyList<LInflectionRule> LInflectionBookFolds, LInflectionLayout? LInflectionBookLayout, string LInflectionBookStamp)`

A language pack's inflection rules, which predict regular forms and mark irregular letters.
Conjugation templates and rewrites remain language-pack data rather than engine branches.
It reads strings and codes only, with no storage behind it.

**Parameters**

- `LInflectionBookKinds`: headword classes in book order, with the first matching templated class winning.
- `LInflectionBookStems`: paradigm rows with their templates and endings.
- `LInflectionBookRules`: ordered rewrites turning a raw stem and ending into a predicted form.
- `LInflectionBookFolds`: comparison rewrites consumed by `LInflectionDifference`, not by prediction.
- `LInflectionBookLayout`: presentation layout, or null when the pack supplies none.
- `LInflectionBookStamp`: source identity used to detect stale stored analyses.

## `public string? LInflectionBookResolve(string headword, IReadOnlyList<long> codes)`

The text of `LInflectionBookDivide` with the root discarded, so prediction keeps one pipeline.

## `public string? LInflectionBookDivide(string headword, IReadOnlyList<long> codes, out int? root)`

Predicts the regular form of `headword` for the cell `codes`, given in any order.
It also gives where the predicted root ends through `root`, so a view can split root from ending.
The root boundary is the `·` a pack template writes after the root.
While a rule leaves the `·` in place, the boundary follows its new index.
A rule that drops it maps the boundary through that rule's matches in order.
A boundary inside a match lands where the match's replacement starts.
A boundary after a match shifts by the length change of the matches before it.
The root is null when the applied template writes no `·`, or when prediction returns null.
The answered root is clamped to the predicted text.
Cell matching compares sorted value sequences, preserving duplicate counts rather than treating codes as a set.
The first matching stem-ending pair wins, even if its missing template makes prediction return null.
The template comes from the first matching kind, in book order, that the stem names.
Null means no kind matches, no cell matches, or no matching kind supplies that stem's template.
Kind-restricted rewrites consider every matching headword kind, not only the template's kind.

## `private IReadOnlyList<LInflectionKind> LInflectionBookRead(string headword)`

Every kind `headword` belongs to, in book order.
