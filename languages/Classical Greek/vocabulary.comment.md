# vocabulary.json

## file

Display vocabulary for Classical Greek.
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
The noun paradigm asks for the genitive singular, which fixes the declension and stem.
A proper noun is excepted.
The verb paradigm asks for the five further principal parts after the present, in dictionary order.
They are the future, aorist, perfect active, perfect middle and aorist passive.
A slot the verb lacks is left empty.
The adjective paradigm asks for the feminine and neuter nominative, which a two-termination adjective repeats.
No `regular` predictions are declared.
A Greek form is a fact of its stem and its accent rather than of its spelling.
