# vocabulary.json
Hash: `e267aab5c680e576`

## file

Display vocabulary for Cantonese.
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
