# TEngineFanqieRank.cs
Hash: `2058eba04117aa36`

## `public sealed class TEngineFanqieRank`

Covers the rank press that marks a fanqie row as a representative, over the wiki pack with stubbed pages.
It builds through `TEngineFanqie.TFanqieSettle` and `TFanqieDraftCreate`.
The pack and the wiki page come from `TEngineFanqieSource`, and the Broad page from `TEngineFanqie`.
A rank press stores the rank the clerk resolves from the held rank and the raise flag.
A press on an unmarked row appends it, and a raising press moves a marked row up.
A plain press on a marked row unmarks it.

## `private static List<int> TFanqieRankRead(LEngine engine, long entryId, IReadOnlyList<long> ids)`

Reads the stored rank of each row in the given id order.
