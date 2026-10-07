# vocabulary.json
Hash: `59154178fa3f9d0d`

## file

Display vocabulary for Classical Latin.
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

It lists the parts of speech, each a bare role.
Valency and other subtypes belong to the dependence field and the morphology, not here.
A part may carry `parent`, the id of the part it specialises.
A paradigm declared on the parent applies to it.

## `retired`

It lists the parts once published and since removed, so no id is ever reused.
The import applies every row on each start, and a row whose part is already gone does nothing.
`into` names the part that takes over the removed part's links.
`unit` names the lexical unit set on a linking entry that has none yet.
A row with neither drops the links.

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
The verb paradigm asks for the three further principal parts, infinitive, perfect and supine, in dictionary order.
A deponent verb writes its perfect participle in the supine slot.
The adjective paradigm asks for the feminine and neuter nominative.
A third-declension adjective of one termination repeats it.
No `regular` predictions are declared.
A Latin form is a fact of its stem rather than of its spelling.
