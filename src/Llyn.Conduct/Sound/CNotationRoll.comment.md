# CNotationRoll.cs

## `public sealed record CNotationRoll(`

The notation popup's state, ready to paint in one go.
The errand answers it from the start gate, from the flag load and with each search event.

**Parameters**

- `CNotationRollRows`: the source rows in the pack's order, one per source.
- `CNotationRollEmpty`: whether no source has a row yet, so the notice shows instead of the list.
- `CNotationRollSearching`: whether the search still runs, so the progress line shows.
- `CNotationRollNotice`: the notice key, searching while the search runs and empty once it ended.
