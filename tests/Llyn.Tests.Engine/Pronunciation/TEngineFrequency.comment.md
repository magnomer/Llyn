# TEngineFrequency.cs
Hash: `1d4be01c6335f7b0`

## `public sealed class TEngineFrequency`

Covers the engine's frequency fill over a fixture pack with three sources.
Every source is asked, and each one that answers stores a row, in written order.
A source that stays silent leaves no row while the others still store theirs.
A source with a unit stamps it on numeric rows only.
A source without a `once` estimate leaves the interval null.
The grading of a stored row is in `TEngineFrequencyBand`, the asking again within a session in `TEngineFrequencySession`.
The fill following its entry is in `TEngineFrequencyEntry`.
The pack, the observer, the draft and the stored-row helpers are shared by all three siblings.
A fill is observed as the user sees it.
The entry is saved, the bulletin lands, and the store is read.

## `internal const string TEngineFrequencyPack`

The fixture pack's JSON with three sources.
`First` has a `once` total and band patterns, `Second` a `once` factor, and `Third` a unit and no bands.

## `internal static readonly TimeSpan TEngineFrequencyPatience`

How long a test waits for a bulletin or a request count before it fails.

## `internal static LEntryDraft TFrequencyDraftCreate(string headword, string language)`

Builds a draft with one plain card, so the siblings save the same entry.

## `internal static void TFrequencyStoredSet(TWorkspace workspace, long entryId, string source, string raw, string? band)`

Writes one frequency row straight into the database, with a null band when `band` is null.

## `internal static long TFrequencyCountRead(TWorkspace workspace, long entryId)`

Counts the frequency rows stored for the entry.

## `internal sealed class TFrequencyObserver`

Completes a task when the first frequency bulletin arrives, and ignores every other subject.

## Inline notes

### `private static async Task<IReadOnlyList<LFrequency>> TFrequencyFetchRead(LEngine engine, string language)`

Saves an entry, waits for the fill its save starts, and reads what the fill stored.
