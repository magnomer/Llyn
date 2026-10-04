# LSettingsPort.cs
Hash: `f8375ac840b99fde`

## `public interface LSettingsPort`

The slice of the engine the window and the settings panel see.
It reads and saves the settings, the workspace path, the workspace state and the interface language.
It also words a failure into a notice, and loads fonts and flags.
The interface texts are read through it, so a deportment names no localization.
Nothing here touches an entry or a draft.
`LSettingsOutlet` implements it today, and a settings clerk takes it over when the parts are dismantled.

## `string LEngineWorkspaceFormat();`

The workspace folder's own name for the settings ledger, or the full path when the root has none.

## `bool LEngineWorkspaceCheck(string chosen);`

Whether the raw `chosen` path names another workspace, so Conduct asks nothing for a blank or unchanged one.

## `LWorkspaceState LEngineWorkspaceChange(string chosen);`

Moves the engine onto the raw `chosen` folder, records it, sweeps it and answers its state.
Conduct changes the workspace in one call, without the engine itself.

## `LWorkspaceState LEngineWorkspaceStart();`

Opens the current workspace for the session in one call.
It sweeps leftover drafts and recordings first, then answers the workspace state.

## `void LEngineFolderOpen();`

Opens the workspace folder in use through the shell usher.
Conduct opens it in one call, without reading the path first.

## `IReadOnlyList<string> LEngineGroupFind(IReadOnlyList<(string, IReadOnlyList<string>)> groups, string? text);`

The names of the groups with a key whose interface text reads `text`, by the localization's shared match.

## `IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language);`

The interface strings of one language, parsed and ready for the resource dictionary.

## `string LEngineLocalizationRead();`

The stored interface language normalised to a listed one.

## `IReadOnlyList<string> LEngineLocalizationScan();`

The interface languages the build embeds, so the language box lists no language named in markup.

## `string LEngineTextRead(string key);`

The interface text under `key`, or the key itself when none is loaded.

## `string? LEngineTextFind(string key);`

The interface text under `key`, or null when none is loaded, so a caller can tell a missing text.

## `void LEngineLocalizationSave(string language);`

Persists the chosen interface language, writing only that field so a switch flipped elsewhere survives.
A settings bulletin follows when the language changed, so the shell applies the catalog.

## `void LEngineFrequencySave(bool frequency);`

Persists whether an entry's frequency is fetched from the pack's web source.
The next fill reads the switch, so a flip neither refetches what is stored nor drops it.

## `void LEngineMorphologySave(bool morphology);`

Turns the morphology fetch on or off.
Turning it off cancels every pending inflection fetch.

## `void LEngineRespellingSave(bool respelled);`

Persists whether readings show and edit in their respelled form.
Every reading is stored in both forms, so a flip only changes which one each surface shows.

## `(string LEngineFailureNotice, string? LEngineFailureLabel, string? LEngineFailurePath)`

The ready notice of a failure, which is a refusal's reason key or `unexpected` with `recorded` and the audit file.
One call reads the reason and writes the fault, so Conduct decides nothing about the exception.

## `LFont LEngineFontRead(string language, LFontRole role);`

The typography the pack declares for one role, or a blank font so the theme's own stands.
The glyph role falls back to the example typography.

## `Task<IReadOnlyList<string>> LEngineEnsignLoad(Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)`

Fetches the flag of every language the cache has not yet asked, then hands the newly kept rows to `store`.
It answers the loaded languages it read, so a language menu fills from the same load.
A fill that a workspace change outran records nothing and calls nothing.
It resumes on the caller's context, so `store` runs on the shell's thread.

## `LEstablishment LEngineEstablishmentRead();`

Counts the held drafts that differ from their origin, the stored entries and the database bytes.
Each held draft is measured as the leave dialog measures it, so the two cannot disagree.

## `IReadOnlyList<string> LEngineLanguageRead();`

The names of the languages with a pack on disk.
The engine keeps the list until the workspace changes, since a fresh scan per entry switch stalled the UI.

## `static string LEngineEnsignFormat(string language, string variety)`

The key a variety's flag is kept under, through the ensign's one key format.
A blank variety has no flag, so its key is empty and finds nothing.
