# vocabulary.json
Hash: `897619964cbdb41c`

## `language`

Display vocabulary for Spanish.
It lists the parts of speech an entry may carry as presets, and the morphology each of them takes.
A preset is what the part-of-speech field offers in its dropdown.
A user may still type a part of speech no preset names, which is stored as typed.
Every id is an integer that never changes once published.
Renaming a preset keeps its id, so every entry linking it follows the rename.
The names are what the shell shows.
Order is the order of these lists.

## `exampleOrder`

It states which of the two frame fields an example row writes first, `particle` or `dependence`.
It states nothing either may hold.

## `registers`

It lists the registers the shell offers on a card.
A user may still write a register no preset names.

## `parts`

It lists the parts of speech, each a bare role.
Valency and other subtypes belong to the dependence field and the morphology, not here.
This pack declares ten parts without parent links.

## `retired`

It lists the parts once published and since removed, so no id is ever reused.
The import applies every row on each start, and a row whose part is already gone does nothing.
`into` names the part that takes over the removed part's links.
`unit` names the lexical unit set on a linking entry that has none yet.
A row with neither drops the links.

## `features`

It lists the grammatical features each part takes.
`part` names the part id.

Feature 9 `polarity` splits the imperative into affirmative and negative rows.
Feature 10 `form` splits the imperfect subjunctive into its -ra and -se rows.
Both belong to the verb, part 6.

## `values`

Value identifiers link each grammatical value to its feature.

Values 22 `affirmative` and 23 `negative` belong to polarity.
Values 24 `-ra` and 25 `-se` belong to form.
The names are what a slot shows, so the hyphen in `-ra` and `-se` is kept.

## `paradigms`

It lists the forms a part is expected to inflect into.
`part` names the part id.
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
