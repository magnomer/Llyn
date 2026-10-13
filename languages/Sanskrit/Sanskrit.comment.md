# Sanskrit

The Sanskrit language selector pack and its display vocabulary.

## `source.json`

Pronunciation and audio sources are listed separately.
Either list can be filled without changing the application.

## `vocabulary.json`

The presets supply part of speech labels for entries.
People can still type labels absent from the lists.
Each id is permanent after publication, and renaming a preset keeps its id.
The shell shows the names in list order.
No inflection features or values are preset yet.

## `registers`

These are the registers the shell offers on a card.
People can still write a register absent from the list.

## `retired`

It lists the parts once published and since removed, so no id is ever reused.
The import applies every row on each start, and a row whose part is already gone does nothing.
`into` names the part that takes over the removed part's links.
`unit` names the lexical unit set on a linking entry that has none yet.
A row with neither drops the links.

## Stamp

Commit: `c2f335c331fe`
- `source.json` `b1a80a2778834aa9`
- `vocabulary.json` `7ad923810052b9ac`
