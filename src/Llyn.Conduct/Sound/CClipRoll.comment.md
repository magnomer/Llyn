# CClipRoll.cs
Hash: `71c16169b341a872`

## `public sealed record CClipRoll(IReadOnlyList<CClipItem> CClipRollRows, bool CClipRollEmpty, bool CClipRollSearching, string CClipRollNotice)`

The clip popup's state, ready to paint in one go.
The errand answers it from the start gate, from the flag load and with each search event.

**Parameters**

- `CClipRollRows`: the source rows in the pack's order, one per source.
- `CClipRollEmpty`: whether no source has a row yet, so the notice shows instead of the list.
- `CClipRollSearching`: whether the search still runs, so the progress line shows.
- `CClipRollNotice`: the notice key, searching while the search runs and empty once it ended.
