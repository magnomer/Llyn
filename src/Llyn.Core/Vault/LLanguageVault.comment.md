# LLanguageVault.cs

## `public interface LLanguageVault`

The port for language packs, the per-language definitions the build ships beside the program.
The engine asks by language name and never learns where a pack sits or how it is written.
`LLanguageLoader` in Infrastructure is its adapter over the `languages` folder.

## `IReadOnlyList<string> LLanguageScan();`

The names of every listed pack, the primary language first and the rest in ordinal order.

## `LLanguage LLanguageRead(string language);`

The pack of `language`, or a blank pack of that name when none is readable.

## `bool LLanguageNameValidate(string? language);`

Whether `language` could name a pack at all.
A name that would escape the pack folder, or name no folder, is refused before any pack is looked for.

## `Task<string?> LLanguageFlagRead(string code, CancellationToken cancellation);`

The local path of the flag image for `code`, an ISO 3166-1 alpha-2 country code.
The adapter fetches it into the workspace on first use and serves the cached copy after.
It answers `null` when the fetch fails, so a missing flag never blocks the UI.
