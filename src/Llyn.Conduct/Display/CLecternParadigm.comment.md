# CLecternParadigm.cs
Hash: `3df03bf13a16f5b7`

## `public sealed record CLecternParadigm(IReadOnlyList<CParadigmSlot> CLecternParadigmSlots, CFont CLecternParadigmFont, CParadigmView? CLecternParadigmView = null)`

The paradigm block of the reading view for the shown entry, ready to show.

**Parameters**

- `CLecternParadigmSlots`: the first slot of every paradigm row, each with its ready text and tip key.
- `CLecternParadigmFont`: the headword typography of the paradigm's own language.
- `CLecternParadigmView`: the inflection box of the entry, or null when no layout or slot gives one or the read fails.
