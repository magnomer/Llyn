# LEnsignLoader.cs
Hash: `bbe110761a561bde`

## `internal sealed class LEnsignLoader`

Fetches a language pack's flag image and caches it under the workspace.
A network fetch is a different job from parsing a pack, so it sits apart from `LLanguageLoader`.
`LLanguageLoader` builds it and answers the vault's flag read through it.

## `public LEnsignLoader(string root, HttpClient client)`

Binds the fetch to the workspace `root` its flag cache lives under and the `client` that fills it.
`LLanguageLoader` checks both arguments before it builds this loader.

## `public async Task<string?> LEnsignFileRead(string code, CancellationToken cancellation)`

Returns the local path to the flag image for `code`, an ISO 3166-1 alpha-2 country code.
It downloads it from the flag-icons set into the workspace's `flags` folder on first use.
It serves the cached copy thereafter.
Returns `null` when the download fails or the client times out, so a missing flag never blocks the UI.
It returns `null` as well when the flag folder or file cannot be made, read or written.
The caller then draws the name without its flag, as for a failed download.
Any other fault still reaches the caller.
A cancellation the caller requested still propagates.

## `private static string LEnsignCodeNormalize(string code)`

The code made safe as one lowercased file name segment, the leaf the flag set names its files by.

## Inline notes

### `private const string LEnsignAddress = "https://cdn.jsdelivr.net/gh/lipis/flag-icons/flags/4x3/";`

The flag-icons set (github.com/lipis/flag-icons), served over jsDelivr's CDN of the repo.
The 4x3 SVGs match the flag box's aspect.
The leaf is the lowercased ISO 3166-1 alpha-2 code.

### `private const string LEnsignSuffix = ".svg";`

The extension of a fetched and cached flag file.
`LLanguageLoader` keeps its own `.svg` key for a pack-shipped emblem, because each type owns its own key.
