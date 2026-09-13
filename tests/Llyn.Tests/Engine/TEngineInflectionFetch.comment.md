# TEngineInflectionFetch.cs

## `public sealed class TEngineInflectionFetch`

Covers the engine's inflection fetch over the shipped English pack against a stub client.
The stub body is shaped as the Wiktionary REST HTML the pack's morphology source reads.
A body naming both verb forms leaves both slots specified and raises the inflection bulletin once.
A body naming only the past leaves the past participle unknown rather than unspecified.
A source that cannot be reached leaves both slots unspecified and raises nothing.
With the setting off, a start asks nothing and writes nothing.
A headword change while a fetch is pending discards the forms instead of storing them under the new headword.
A hand set of the inflections forgets the unknown mark, so the slot reads unspecified again.

## Inline notes

### `public async Task InflectionStart_BothFormsFound_MarksSlotsSpecifiedRaisesOnce()`

The frequency fill is switched off first, so the stub client only ever sees the morphology request.
The short wait after the bulletin lets a second bulletin land if the fetch wrongly raised twice.

### `public async Task InflectionStart_SettingOff_AsksNothing()`

The setting is written to the workspace before the engine starts, since the engine reads it on construction.

### `public async Task EntryUpdate_HeadwordChangedMidFlight_DiscardsForms()`

The client holds its answer behind a gate, so the headword changes under the pending fetch.
Nothing observable follows, so a short wait stands in for the fetch settling.

### `private static async Task TInflectionCountCheck(TSourceHandler handler, int count)`

Polls the handler until it has seen `count` requests or the patience runs out.
The short delay that follows in the caller lets the fetch settle after its request.
