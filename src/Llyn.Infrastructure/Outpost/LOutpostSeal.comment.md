# LOutpostSeal.cs
Hash: `29af3e9a4ce34b26`

## `internal static class LOutpostSeal`

The one id check every Joplin call goes through before it builds a URL.
It lives apart so the HTTP adapter stays short.

## `public static bool LOutpostSealMatch(string? id)`

Whether `id` is exactly 32 lowercase hex characters, the only form of Joplin id Llyn writes.
A null `id` is not a match.

## `public static void LOutpostSealCheck(string id)`

Throws `ArgumentException` when `LOutpostSealMatch` answers false for `id`.
That is the only form of id Joplin hands out or Llyn derives.
Any other text could add a path segment or query and reach an item Llyn never created.
