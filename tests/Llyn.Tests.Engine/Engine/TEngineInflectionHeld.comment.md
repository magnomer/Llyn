# TEngineInflectionHeld.cs
Hash: `304f7cba66a64d54`

## `public sealed class TEngineInflectionHeld`

Covers the inflection fetch of an entry an editor holds unchanged right after its save.
The body is the Spanish conjugation table cut from the real Wiktionary REST page for `vivir`.
Its long lines are wrapped only where the shipped readings allow white space.
The shipped Spanish pack reads all 64 cells from it.

## `public async Task InflectionStart_SavedEntryHeldUnchanged_FillsEveryCell()`

The client holds its answer behind a gate, so the save's fetch is still pending when a draft is held.
That draft is the editor reopening on the saved entry, so it matches the stored entry.
The reading view then starts the entry's fills, as opening it does.
Every cell of the box reads a form, the store keeps 64 forms, and the held draft carries them too.

## `private static async Task TInflectionHeldSettle(LEngine engine, TSourceHandler handler, long entryId)`

Waits until the client was asked and no fetch stands for the entry.
