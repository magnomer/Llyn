# TLibraryMarkup.cs
Hash: `d89d323771ed9d3f`

## `public sealed class TLibraryMarkup`

Covers the library panel's markup import on a real workspace.
It builds its panel through `TLibrary.TLibraryPrepare` and its envoy through `TLibraryEnvoyCreate`.
The markup import asks for the file first, and a cancelled pick asks nothing more.
It then asks the customs question once and stores under the declared rows.
The question shows each entry with its ready targets, and a clean file reports nothing after.
A merge row appends to its target, so the declared mode reaches the engine unchanged.
A declined question stores nothing, and a malformed file shows the import failure.

## `private static CEnvoy TLibraryEnvoyCreate(string? file, Func<CSCustoms, bool> customs, List<string> asked)`

An envoy that answers the file question with `file` and the customs question through `customs`.
It records every question it is asked.
`customs` drives the handed gate as a window would, then answers whether the user accepted.
