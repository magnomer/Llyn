# LParadigm.cs
Hash: `854b48d527c8e752`
Hash: `0c6613f3fd9044a1`

## `public static string LParadigmLanguageRead(IReadOnlyList<LParadigmSlot> slots)`

The language the first slot's speech carries, or empty when there are no slots, for the section's font.

## `public sealed record LParadigm(long LParadigmSpeechCode, IReadOnlyList<IReadOnlyList<long>> LParadigmCells, IReadOnlyList<LParadigmRule>? LParadigmRegular = null, IReadOnlyList<long>? LParadigmExcept = null)`

The forms one part of speech is expected to inflect into, as a language pack declares them.
A pack states a paradigm on a part by code, so it is never stored and has no row id.
A paradigm declared on a parent part applies to every part that names that parent.
A part listed in `except` stops that reach, for itself and for every part under it.
Each form is a cell, a set of morphology value codes asked for together, such as mood, tense and person.
A value the pack lists alone is a cell of one value, ahead of the cells it states.
That order is the order the forms are shown and asked for.
Optional rules say which spellings of a form are regular.
Each rule that matches the headword predicts one spelling, and a form equal to any prediction needs no note.

**Parameters**

- `LParadigmSpeechCode`: Code of the part of speech the paradigm belongs to.
- `LParadigmRegular`: Rules predicting the regular spellings of a form from the headword, empty when the pack states none.
- `LParadigmExcept`: Codes of the parts the paradigm must not reach through the parent chain, empty when none.
- `LParadigmCells`: The forms as cells of value codes in display order.

## `public IReadOnlyList<LParadigmRule> LParadigmRegular { get; init; }`

An omitted rule list becomes empty, so regularity checks need no null branch.

## `public IReadOnlyList<long> LParadigmExcept { get; init; }`

Omitted exceptions become empty, preserving the full parent-chain reach.
