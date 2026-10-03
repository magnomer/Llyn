# TEngineFrequencyEntry.cs
Hash: `7c9da840e11489cf`

## `public sealed class TEngineFrequencyEntry`

Covers how the fill follows the entry it was started for.
With the setting off, a start writes nothing and raises no bulletin.
A headword change clears the stored rows at once and refetches under the new headword.
A fill whose entry was deleted or whose language changed while its answer was pending writes nothing.
An entry with a blank language reads empty and starts no fill.
It observes the fill through `TEngineFrequency.TFrequencyObserver` and builds its entry through `TEngineFrequency.TFrequencyDraftCreate`.
It writes and counts rows through the stored-row helpers of `TEngineFrequency`.

## Inline notes

### `public async Task EntryUpdate_HeadwordChanged_ClearsStoredRowsThenRefetches()`

The client holds its answer behind a gate, so the cleared store is observable before the refetch lands.
Releasing the gate lets the fill finish, and the bulletin marks the moment the new row is stored.

### `public async Task EntryUpdate_LanguageChangedMidFlight_DropsOldAnswer()`

The second fixture pack is empty, so the new language starts no fill of its own.
Only the stale fill could write, and the assertion is that it does not.

### `public async Task FrequencyStart_EntryDeletedMidFlight_WritesNothing()`

The same gate holds the fill open while the entry is deleted under it.
Nothing observable follows, so a short wait stands in for the fill settling.
