# TEngineFrequency.cs
Hash: `809864258bdb9ebb`

## `public sealed class TEngineFrequency`

Covers the engine's frequency fill over a fixture pack with three sources.
Every source is asked, and each one that answers stores a row, in written order.
A source that stays silent leaves no row while the others still store theirs.
A numeric raw is graded by interval from the source's `once` estimate, ahead of its band patterns.
A raw that is not a number is graded by the first band pattern it matches.
It is graded null when none does.
A source with a unit stamps it on numeric rows only.
A source without a `once` estimate leaves the interval null.
A blank language resolves no band at all.
A stored band is regraded from the raw at read time and written back when it differs.
A row stored with no band, as a migrated one is, is resolved and stored on its first read.
With the setting off, a start writes nothing and raises no bulletin.
A headword change clears the stored rows at once and refetches under the new headword.
A fill whose entry was deleted while its answer was pending writes nothing and raises nothing.
An entry with a blank language reads empty, starts no fill, and throws nothing.
A stored row on such an entry still reads back, only with its band and interval left null.
A language change while a fill is pending drops the old answer instead of storing it under the new language.
A word every source reached yet none knew is asked once per session, however often it is read.
A word no source could be reached for is asked again on the next read.
A fill is observed as the user sees it.
The entry is saved, the bulletin lands, and the store is read.

## Inline notes

### `public async Task EntryUpdate_HeadwordChanged_ClearsStoredRowsThenRefetches()`

The client holds its answer behind a gate, so the cleared store is observable before the refetch lands.
Releasing the gate lets the fill finish, and the bulletin marks the moment the new row is stored.

### `public async Task EntryUpdate_LanguageChangedMidFlight_DropsOldAnswer()`

The second fixture pack is empty, so the new language starts no fill of its own.
Only the cancelled fill could write, and the assertion is that it does not.

### `private static async Task TFrequencyCountCheck(TSourceHandler handler, int count)`

Polls the handler until it has seen `count` requests or the patience runs out.
The short delay that follows in the callers lets the fill settle after its last request.

### `public async Task FrequencyStart_EntryDeletedMidFlight_WritesNothing()`

The same gate holds the fill open while the entry is deleted under it.
Nothing observable follows, so a short wait stands in for the fill settling.

### `private static async Task<IReadOnlyList<LFrequency>> TFrequencyFetchRead(LEngine engine, string language)`

Saves an entry, waits for the fill its save starts, and reads what the fill stored.

### `private static string? TFrequencyBandRead(`

Stores one raw value with no band for `source` and reads back the band the pack grades it into.
The entry's earlier rows are dropped first, so each call reads exactly one row.
