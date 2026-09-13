# TEngineFrequency.cs

## `public sealed class TEngineFrequency`

Covers the engine's frequency fill over a fixture pack with two sources and three bands.
The fill takes the first non-empty answer in written order, whichever source answers first.
A band is the first pack row the raw satisfies, by limit or by pattern, and null when none does.
A blank language resolves no band at all.
A stored value reads back with its band resolved from the pack at read time, not from the store.
With the setting off, a start writes nothing and raises no bulletin.
A headword change clears the stored value at once and refetches under the new headword.
A fill whose entry was deleted while its answer was pending writes nothing and raises nothing.
An entry with a blank language reads null, starts no fill, and throws nothing.
A stored value on such an entry still reads back, only with its band left null.
A language change while a fill is pending drops the old answer instead of storing it under the new language.
A word every source reached yet none knew is asked once per session, however often it is displayed.
A word no source could be reached for is asked again on the next display.
A source that answers stops the fill, so the later source is never asked.

## Inline notes

### `public async Task EntryUpdate_HeadwordChanged_ClearsStoredValueThenRefetches()`

The client holds its answer behind a gate, so the cleared store is observable before the refetch lands.
Releasing the gate lets the fill finish, and the bulletin marks the moment the new value is stored.

### `public async Task EntryUpdate_LanguageChangedMidFlight_DropsOldAnswer()`

The second fixture pack is empty, so the new language starts no fill of its own.
Only the cancelled fill could write, and the assertion is that it does not.

### `private static async Task TFrequencyCountCheck(TSourceHandler handler, int count)`

Polls the handler until it has seen `count` requests or the patience runs out.
The short delay that follows in the callers lets the fill settle after its last request.

### `public async Task FrequencyStart_EntryDeletedMidFlight_WritesNothing()`

The same gate holds the fill open while the entry is deleted under it.
Nothing observable follows, so a short wait stands in for the fill settling.
