# vocabulary.json

## file

Display vocabulary for French.
It lists the parts of speech an entry may carry as presets, and the morphology each of them takes.
A preset is what the part-of-speech field offers in its dropdown.
A user may still type a part of speech no preset names, which is stored as typed.
Every id is an integer that never changes once published.
Renaming a preset keeps its id, so every entry linking it follows the rename.
The names are what the shell shows.
Order is the order of these lists.
Add a language by creating `languages/<Name>/vocabulary.json`.

## `exampleOrder`

It states which of the two frame fields an example row writes first, `particle` or `dependence`.
It states nothing either may hold.

## `registers`

It lists the registers the shell offers on a card.
A user may still write a register no preset names.

## `parts`

It lists the parts of speech.
A part may carry `parent`, the id of the part it specialises.
A paradigm declared on the parent applies to it.

## `features`

It lists the grammatical features each part takes.
`part` names the part id.

## `values`

It lists the values each feature takes.
`feature` names the feature id.

## `paradigms`

It lists the forms a part is expected to inflect into.
`part` names the part id and `values` the morphology value ids in display order.
`except` lists part ids on the parent chain the paradigm must not reach.
The optional `regular` is a list of `[pattern, replacement]` pairs.
Each pair whose pattern matches the headword predicts one regular spelling by replacing the first match.
A stored form equal to any prediction is regular and needs no note.
The noun paradigm predicts the plural.
A headword ending in s, x or z stays.
One ending in eau, au or eu takes x.
One ending in al takes aux, and any other takes s.
The adjective paradigm predicts the feminine singular, the masculine plural and the feminine plural.
The feminine keeps a final e.
It turns f into ve, x into se, er into ère, et into ète or ette, eau into elle.
It turns teur into trice, eur into euse, c into che or que and g into gue.
It doubles the consonant of a final el, en, il or on, and otherwise adds e.
The masculine plural follows the noun rules.
The feminine plural is the feminine with s.
The verb paradigm predicts the present and past participles from the infinitive ending.
er gives ant and é, with ger giving geant and cer giving çant.
ir gives issant and i.
re gives ant and u.
A form no pair predicts, such as yeux from œil, is irregular and carries a note.
