# LParadigm.cs

## `public sealed record LParadigm(`

The forms one part of speech is expected to inflect into, as a language pack declares them.
A pack states a paradigm on a part by code, so it is never stored and has no row id.
A paradigm declared on a parent part applies to every part that names that parent.
A part listed in `except` stops that reach, for itself and for every part under it.
The values are morphology value codes in the order the pack lists them.
That order is the order the forms are shown and asked for.
Optional rules say which spellings of a form are regular.
Each rule that matches the headword predicts one spelling, and a form equal to any prediction needs no note.

**Parameters**

- `LParadigmSpeechCode` — Code of the part of speech the paradigm belongs to.
- `LParadigmMorphology` — Codes of the morphology values the part inflects into, in display order.
- `LParadigmRegular` — Rules predicting the regular spellings of a form from the headword, empty when the pack states none.
- `LParadigmExcept` — Codes of the parts the paradigm must not reach through the parent chain, empty when none.
