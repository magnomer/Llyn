# LSpeechPack.cs

## `public sealed record LSpeechPack(`

The display vocabulary one language pack declares.
It lists the parts of speech the language uses.
It also lists the morphology features each of those parts of speech takes.
Loaded from `languages//vocabulary.json` and written into `part_of_speech_value` and `morphology_value`.
Those tables are keyed by language.
So a pack is the only place a language's vocabulary is stated, and no such fact is compiled in.

The two lists travel together because the morphology rows are keyed by their part of speech.
Reading one without the other leaves rows that name a part of speech nothing declared.

**Parameters**

- `LSpeechPackValues` — The parts of speech, in the order the pack lists them.
- `LSpeechPackMorphology` — The morphology rows, in the order the pack lists them.
