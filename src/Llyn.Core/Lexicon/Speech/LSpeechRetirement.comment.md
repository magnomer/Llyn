# LSpeechRetirement.cs
Hash: `f7f395a97e64a7e0`

## `public sealed record LSpeechRetirement(long LSpeechRetirementCode, long LSpeechRetirementTarget, LUnit LSpeechRetirementUnit)`

One part of speech a language pack once published and has since removed.
A pack never reuses a published code, so it keeps the code here and says where its links go.
The import applies every retirement on each start, and one already applied finds nothing to do.

**Parameters**

- `LSpeechRetirementCode` — The pack code of the removed part.
- `LSpeechRetirementTarget` — The pack code of the part that takes over its links, or `0` to drop them.
- `LSpeechRetirementUnit` — The lexical unit set on a linking entry that has none yet, or `LUnitEmpty`.
