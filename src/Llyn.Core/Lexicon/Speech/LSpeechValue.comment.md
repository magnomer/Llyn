# LSpeechValue.cs
Hash: `952a748c7cb55732`

## `public sealed record LSpeechValue(long LSpeechValueId, string LSpeechValueLanguage, long LSpeechValueCode, string LSpeechValueName, int LSpeechValuePosition, long LSpeechValueParent = 0)`

One row of a language's part-of-speech vocabulary.
Lexical rows link it by `LSpeechValueId` and never copy its name.
A pack row carries the pack's own number in `LSpeechValueCode`.
Re-seeding a pack upserts on `(language, code)`, so a renamed preset keeps its row id.
A value the user added from the editor carries a negative code, which no pack can collide with.

**Parameters**

- `LSpeechValueId` — Row id, `0` before the row is stored.
- `LSpeechValueLanguage` — Language the value belongs to.
- `LSpeechValueCode` — Number the pack file gave the value, negative for user-added values.
- `LSpeechValueName` — Display name for the language (for example `Noun`).
- `LSpeechValuePosition` — Stored order, filled by arrival, and never used to order a view.
- `LSpeechValueParent` — Pack code of the part this one specialises, `0` when none, carried from the pack and never stored.
