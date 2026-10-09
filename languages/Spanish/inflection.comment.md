# inflection.json
Hash: `0749a67011af5ab8`

Declares how a Spanish verb's regular forms are predicted and how the inflection box lays them out.
Spanish conjugation rules and inflection layouts remain pack data rather than engine branches.
Any edit changes the book's stamp, and stored verbs are analysed again from their stored forms.

## `kinds`

Each kind names a headword class by a regex, matched case-insensitively.
A headword belongs to every kind whose regex matches.
`a` covers the -ar verbs, and `other` covers the -er and -ir verbs.
`ir` also covers the -ir verbs, and only limits rules, since no stem names it.
The named groups `root` and `vowel` feed the templates.
The kind names are ASCII pack identifiers and are never shown.

## `columns`

The six person and number cells, first singular to third plural, as value codes.
Each stem's endings are read against these in order.

## `stems`

One row per tense, named by its mood, tense and form value codes.
`templates` gives the stem per kind, written with `·` after the root and `+` before the ending.
The template of the first matching kind in book order is used.
`endings` holds one ending per column, and `null` means the cell has no form.
Neither imperative row supplies a first-singular form.

## `rules`

Ordered regex rewrites from the raw stem and ending to the predicted form.
A rule with a third element runs only for headwords of that kind.
`S*` swaps the stem vowel between e and a.
The -ir rule changes the marked stem vowel from e to i before `mos`, `'is`, or `d`.
`O`, `(i)`, `(e)` and `_` merge or drop vowels at the stem boundary.
`'` marks stress only, `∅` is an empty ending, and the last rule drops the markers.

## `folds`

Ordered regex rewrites applied to both the predicted and the stored form before they are compared.
They fold accents, c and qu, z and c, g and gu, g and j.
They also fold i and y between vowels.
Differences removed by these folds do not count as irregularity.

## `layout`

The box drawn for part 6, the verb, in place of the plain paradigm list.
Every `label` and every `headers` entry is a localization key, so the box follows the interface language.
The keys are generic grammar terms that another pack may reuse.
An empty label stays empty and draws nothing.
Each cell combines its line's `values` with explicit cell values or the sheet's column values.
`collapsed` and `expanded` are the default sheets, shown when custom analysis is off.
Both list the indicative, then the subjunctive, then the imperative.
`collapsed` uses first singular forms, both imperfect subjunctive variants, and affirmative imperative second singular.
`expanded` covers the declared rows across six person columns.

## `layout.custom`

`custom` holds the sheets shown when custom analysis is on, with their own `collapsed` and `expanded`.
`custom.collapsed` leads with future and conditional, then the imperative, indicative and subjunctive.
`custom.expanded` follows the same order, but splits future and conditional into two groups.
Its future group holds both the indicative and the subjunctive future.
A pack without `custom` shows its default sheets under either setting.
Marked letters show only while custom analysis is on.
Each custom form also splits its root from its ending where the template writes `·`.
