# Spanish

The Spanish language pack is split over three data files.
`source.json` holds the sources, `inflection.json` the verb rule book, and `vocabulary.json` the display vocabulary.

## `source.json`

The pack identifies Spanish independently of the interface language.
The selector uses the Spain flag, headwords use Segoe UI at size 40, and examples prefer Georgia at size 15.

## `pronunciation[0]` Wiktionary

The es-pr template derives the IPA from the spelling, so the wikitext carries none and the rendered HTML is read.
The match anchors on the Spanish section heading, so a Galician or Asturian section before it is skipped.
The first IPA span after the heading is the phonemic reading between slashes.
The phonetic reading in brackets, the rhyme and the regional readings that follow are left alone.
A word that splits by region, such as ciudad, so gives the distinción reading first.

## `audio[0]` Naver

Naver's text-to-speech answers every headword, since a synthesizer has no missing entries.
The address is the recording, so the `link` strategy answers it once the fetch succeeded.
The speaker is `es_es` and the service `esdic`.

## `audio[1]` Wiktionary

Wiktionary embeds Commons recordings as protocol-relative addresses, so `prefix` supplies the scheme.
Only the transcoded mp3 rendition is taken, because the original is Ogg, Oga or WAV.
The first attempt takes an `Es-` filename after the Spanish section heading.
The filename carries a region code, as Es-am-lat-agua.ogg or Es-bo-casa.oga.
The second takes a Lingua Libre `LL-Q1321_%28spa%29-` one, Q1321 being Spanish.

## `inflection`

The key names the file beside this one holding the verb rule book, `inflection.json`.
The book predicts each regular form and marks the letters where a stored form departs from it.
It is kept apart because its rules and layout are long and change on their own.

## `morphology[0]`

One Wiktionary REST HTML attempt declares 64 cell-keyed readings.
Normalization is disabled so conjugated spelling survives extraction.
The attempt requires `lang-es` on the page, so a page without a Spanish section gives no reading.
The Spanish conjugation sits in one `roa-inflection-table`.
Each finite row opens with a `<th>` holding `<span title="T">`, where `T` is the Spanish row name.
Six `<td>` cells follow, one per person and number, first singular to third plural.
A cell's form is the text of its first `<a>`.
Indicative and subjunctive 2sg cells carry the tú form first and the vos form second.
So the first `<a>` keeps the tú form and drops vos.
The imperative rows come last and keep an empty first `<td>` for the missing first singular.
Every negative imperative cell reads `no` and the form as two `<a>` in one span.
Its reading skips the `no` link and captures the next one.
The two imperfect subjunctive rows share one title and differ by `<br/>(ra)` or `<br/>(se)` after it.
The future subjunctive header ends in a footnote `<sup>`, so a header may carry markup before `</th>`.
Rows after the present indicative first require its title, avoiding earlier tables with reused imperative titles.
These anchors do not explicitly enforce table boundaries.
Each reading takes column k by skipping k cells, and its `variety` is the cell key.
Patterns require exact row-title spellings, including `copréterito`.
Embedded recibir fixtures verify alternate tú forms, negative-particle skipping, and earlier Galician decoys.

## `inflection.json`

The file declares how a Spanish verb's regular forms are predicted and how the inflection box lays them out.
Conjugation rules and layouts remain pack data rather than engine branches.
Any edit changes the book's stamp, and stored verbs are analysed again from their stored forms.

## `kinds`

Each kind names a headword class by a regex, matched case-insensitively.
A headword belongs to every kind whose regex matches.
`a` covers the -ar verbs, and `other` covers the -er and -ir verbs.
`ir` also covers the -ir verbs, and only limits rules, since no stem names it.
The named groups `root` and `vowel` feed the templates.
The kind names are ASCII pack identifiers and are never shown.

## `columns`

These are the six person and number cells, first singular to third plural, as value codes.
Each stem's endings are read against these in order.

## `stems`

There is one row per tense, named by its mood, tense and form value codes.
`templates` gives the stem per kind, written with `·` after the root and `+` before the ending.
The template of the first matching kind in book order is used.
`endings` holds one ending per column, and `null` means the cell has no form.
Neither imperative row supplies a first-singular form.

## `rules`

These are ordered regex rewrites from the raw stem and ending to the predicted form.
A rule with a third element runs only for headwords of that kind.
`S*` swaps the stem vowel between e and a.
The -ir rule changes the marked stem vowel from e to i before `mos`, `'is`, or `d`.
`O`, `(i)`, `(e)` and `_` merge or drop vowels at the stem boundary.
`'` marks stress only, `∅` is an empty ending, and the last rule drops the markers.

## `folds`

These ordered regex rewrites apply to both the predicted and the stored form before they are compared.
They fold accents, c and qu, z and c, g and gu, and g and j.
They also fold i and y between vowels.
Differences removed by these folds do not count as irregularity.

## `layout`

The box is drawn for part 6, the verb, in place of the plain paradigm list.
Every `label` and every `headers` entry is a localization key, so the box follows the interface language.
The keys are generic grammar terms that another pack may reuse.
An empty label stays empty and draws nothing.
Each cell combines its line's `values` with explicit cell values or the sheet's column values.
`collapsed` and `expanded` are the default sheets, shown when custom analysis is off.
Both list the indicative, then the subjunctive, then the imperative.
`collapsed` uses first singular forms, both imperfect subjunctive variants, and affirmative imperative second singular.
`expanded` covers the declared rows across six person columns.

## `layout.custom`

It holds the sheets shown when custom analysis is on, with their own `collapsed` and `expanded`.
`custom.collapsed` leads with future and conditional, then the imperative, indicative and subjunctive.
`custom.expanded` follows the same order, but splits future and conditional into two groups.
Its future group holds both the indicative and the subjunctive future.
A pack without `custom` shows its default sheets under either setting.
Marked letters show only while custom analysis is on.
Each custom form also splits its root from its ending where the template writes `·`.

## `vocabulary.json`

The parts of speech an entry may carry are presets in dropdowns.
A user may still type a value no preset names, which is stored as typed.
Every id is an integer that never changes once published.
Renaming a preset keeps its id, so every entry linking it follows the rename.

## `exampleOrder`

It states which of the two frame fields an example row writes first, `particle` or `dependence`.
It states nothing either may hold.

## `parts`

Valency and other subtypes belong to the dependence field and the morphology, not here.
This pack declares ten parts without parent links.

## `retired`

It lists the parts once published and since removed, so no id is ever reused.
The import applies every row on each start, and a row whose part is already gone does nothing.
`into` names the part that takes over the removed part's links.
`unit` names the lexical unit set on a linking entry that has none yet.
A row with neither drops the links.

## `features`

Feature 9 `polarity` splits the imperative into affirmative and negative rows.
Feature 10 `form` splits the imperfect subjunctive into its -ra and -se rows.
Both belong to the verb, part 6.

## `values`

Values 22 `affirmative` and 23 `negative` belong to polarity.
Values 24 `-ra` and 25 `-se` belong to form.
The names are what a slot shows, so the hyphen in `-ra` and `-se` is kept.

## `paradigms`

`cells` lists each expected form as a set of value ids.
A cell's key is its ids sorted ascending and joined by `+`.
That key is the `variety` of the source reading that fetches the form.
Within a cell the ids follow mood, tense, polarity or form, person, number.
That order becomes the slot's display name.
The verb paradigm holds 64 cells.
The indicative has five tenses of six persons each.
The imperative has two polarities of five persons each.
It has no first person singular, as the source table leaves that column empty.
The subjunctive has present, imperfect -ra, imperfect -se and future, six persons each.
There is no infinitive cell and no vos cell.

## Stamp

Commit: `c2f335c331fe`
- `inflection.json` `0749a67011af5ab8`
- `source.json` `e934d07b2e32cf6a`
- `vocabulary.json` `897619964cbdb41c`
