# LEngineLanguage.cs

## `public sealed partial class LEngine`

The language-pack side of the engine boundary.
Everything the shell asks about a language as such is answered here.
That is the packs on disk, a pack's typography, its flag, and its regional varieties.
Each pack is loaded once through `LLanguageLoader` and kept by name, so no lookup parses the file again.
The cache is cleared with the workspace, since a pack's source lists belong to the folder it was read from.
The shell never reaches into the `languages/` folder itself.

## `public LFont LEngineFontRead(string language)`

The typography the pack declares for a headword, the role most callers want.

## `public LFont LEngineFontRead(string language, LFontRole role)`

The typography the pack declares for one role: the headword, an example line, a gloss, or a glyph chip.
The glyph role falls back to the example typography, so a chip stays serif when the section names no font.
A pack that declares none for the role answers a blank font, and the theme's own typography stands.

## `public IReadOnlyList<string> LEngineLanguageRead()`

Returns the names of the languages that have a pack on disk, for the UI to offer as choices.
The scan opens and parses every pack once, and the engine keeps the list until the workspace changes.
Every panel asks for it on each entry switch, so a fresh scan each time stalled the UI thread.

## `public async Task<string?> LEngineFlagRead(string language, CancellationToken cancellation)`

Returns the local path to the given language's flag image, for the UI to display beside it.
It returns `null` when the pack declares no flag or the download fails.
The pack declares only an ISO country code.
The engine downloads the matching flag from the flag-icons set and caches it in the workspace.
So the UI never reaches into the `languages/` folder itself.

## `internal IReadOnlyList<LVariety> LEngineVarietyRead(string language)`

Returns the regional varieties the language pack declares, in the pack's order.
Empty when the pack declares none, so a language without varieties shows plain rows.
It reads the pack from the engine's cache, so a menu reopened does not reparse the file.

## `internal bool LEngineFlaggedCheck(string language)`

Reports whether the pack asks the UI to label a reading's variety by flag rather than by name.
It reads the cached pack as `LEngineVarietyRead` does.

## `public bool LEngineTonalCheck(string language)`

Reports whether the pack declares the language tonal, so the UI knows to draw a tone contour under a reading.
It reads the cached pack as `LEngineVarietyRead` does.

## `public bool LEngineSilentCheck(string language)`

Reports whether the pack declares the language silent, so the input panel knows to hide its pronunciation rows.
It reads the cached pack as `LEngineVarietyRead` does.
A blank language has no pack and answers false, so an empty desk keeps its pronunciation rows.

## `public async Task<IReadOnlyList<LEnsignRow>> LEngineEnsignLoad()`

Fetches the flag of every language not yet asked of the cache, all at once, and records what came back.
Answers the rows newly kept, so the shell draws only those and asks nothing about the rest.
A fill that a workspace change outran records nothing and answers nothing.

## `public async Task<IReadOnlyList<LEnsignRow>> LEngineEnsignLoad(string language, IEnumerable<string> varieties)`

The same fill for the named varieties of one language, keyed `language/variety`.

## `public void LEngineEnsignDelete(string path)`

Drops a cached SVG the veneer's renderer could not read, through the engine's flag cache and its usher.
The veneer asks here rather than deleting itself, so no file work happens above the engine.

## `private async Task<string?> LEngineEnsignRead(string language)`

One language's flag path, or null when the pack declares none or the fetch failed.
One missing flag never stops the fill.

## `private async Task<string?> LEngineEnsignResolve(string language, string variety)`

One variety's flag path, or null on the same terms.

## `public async Task<string?> LEngineVarietyResolve(string language, string variety, CancellationToken cancellation)`

Returns the local path to the flag image of one named variety of the language.
It returns `null` when the pack does not declare that variety, declares no flag for it, or the download fails.
The name is matched exactly, because it is the tag the pack's own readings carry.

## `private async Task<string?> LEngineFlagResolve(string? code, CancellationToken cancellation)`

The shared tail of both flag entry points.
A null or blank code answers null, any other is fetched through the workspace cache.
A rooted path is a pack's own SVG and answers itself when the file exists, with no fetch.

## `private LLanguage LEngineLanguageLoad(string language)`

The one place a language pack is read, kept by name after the first read.
Every reader of a pack's declarations goes through here, so no path parses the file twice.
It takes the gate itself, and a caller already holding it re-enters without harm.

## `public bool LEngineFlaggedCheck(LEntryDraft draft)`

Reads the language from the displayed draft and treats an empty language as unflagged.
