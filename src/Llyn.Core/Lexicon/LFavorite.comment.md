# LFavorite.cs

## `public sealed record LFavorite(`

An entry the user has marked a favorite, carried with the moment it was marked.
Favorite status is no lexical object, so this record adds nothing to the entry itself.
The entry keeps its identity whether it is marked or not.
The stamp exists so the favorite catalog can order by when the mark was made.

**Parameters**

- `LFavoriteEntry` — The marked entry, unchanged by the mark.
- `LFavoriteMarkedUtc` — Moment the mark was made, ISO 8601 UTC.
