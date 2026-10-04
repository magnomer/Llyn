# LReflexGloss.cs
Hash: `ebf49b306ca16881`

## `public sealed class LReflexGloss`

The user-owned meanings of reflex rows, remembered per entry across a rebuild.
A rebuild clears the rows and fetches them again, which would lose meanings the user typed.
`LReflexFetch` owns one and reads and writes it only under the engine's gate.

## `public void LReflexGlossRecord(long entryId, IReadOnlyList<LReflex> rows)`

Remembers every non-empty user-owned meaning of `rows` before a rebuild clears them.
An entry with no such meaning forgets anything remembered for it before.

## `public IReadOnlyList<LReflex> LReflexGlossRestore(long entryId, IReadOnlyList<LReflex> rows)`

Restores the remembered meanings to matching fetched rows and consumes the remembered set.
A restored meaning is marked user-owned again.

## `public void LReflexGlossClear()`

Forgets every remembered meaning when `LReflexFetchClear` cancels the pending fetches that would have restored them.

## `private static string LReflexGlossFormat(LReflex row)`

The language, region, kind and text key that identifies a reading across a rebuild.
