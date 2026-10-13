# Morphology

This folder holds the grammar vocabulary a language pack declares and the inflected forms an entry stores against it.

## `LFeature`

One grammatical feature a part of speech takes, which owns the `LMorphology` rows naming its values.
Inside a loaded `LSpeechPack`, the speech id holds the part's code, because the row id is unknown until import.

## `LMorphology`

One value a grammatical feature can take.
Inflections link it by id and never copy its name.
Inside a loaded `LSpeechPack` the parent link holds the parent's code, because the row id is unknown until import.

## `LInflection`

One inflected form of an entry, ordered within it.
It has its own row id so its morphology links can name it directly.
It links its part of speech by row id, never by name.
It carries the morphology value ids it shows, in order, and each value knows its feature.
The regular flag is derived when the form or its entry is stored.
So the reading view never runs the pattern.
Covered nonempty forms are regular when the book marks nothing, and uncovered forms fall back to the paradigm's regular patterns.
The prediction is null when no rule covers the cell, and for every pack without a rule book.
The marks are the ranges departing from the prediction.
An empty list means covered with nothing marked, and null means uncovered.
The display reads marks as stored and never runs the rule book.
The stamp names the rule book that made the analysis, including uncovered cells.
A stamp unlike the current book's says the analysis is stale and is made again.

## `LParadigm`

The forms one part of speech is expected to inflect into, as a language pack declares them.
A pack states a paradigm on a part by code, so it is never stored and has no row id.
A paradigm declared on a parent part applies to every part that names that parent.
A part listed in `except` stops that reach, for itself and for every part under it.
Each form is a cell, a set of morphology value codes asked for together, such as mood, tense and person.
A value the pack lists alone is a cell of one value, ahead of the cells it states.
That order is the order the forms are shown and asked for.
Optional rules say which spellings are regular, and a form equal to any prediction needs no note.
An omitted rule list or exception list becomes empty, so checks need no null branch.
`LParadigmLanguageRead` gives the first slot's language, or empty when there are no slots, for the section's font.

## `LParadigmRule`

One rule predicting a regular spelling of an inflected form from the headword.
The pattern is a regular expression, and the replacement is what its first match becomes.
So `([^aeiou])y$` with `$1ies` turns `city` into `cities` and leaves `day` alone.
Several rules may match one headword, and every prediction they make counts as regular.
Matching ignores case and waits at most one second.
`LParadigmRuleResolve` returns null for no match and replaces only the first match, so an end anchor never fires twice.
A match that times out predicts nothing rather than failing the reader.
Equality compares the two strings, so a pack reloaded with the same rules compares equal.

## Stamp

Commit: `c2f335c331fe`
- `LFeature.cs` `6987fd240d92b64c`
- `LInflection.cs` `9717db5220e1d767`
- `LMorphology.cs` `2a3f289bb2192405`
- `LParadigm.cs` `854b48d527c8e752`
- `LParadigmRule.cs` `450b4407b51eabf4`
