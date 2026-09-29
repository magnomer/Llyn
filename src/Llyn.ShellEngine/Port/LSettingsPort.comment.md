# LSettingsPort.cs

## `public interface LSettingsPort`

The slice of the engine the window and the settings panel see.
It reads and saves the settings, the workspace path, the workspace state and the interface language.
It also reads the rescue report, records an audit line, words a refusal, and loads fonts and flags.
The interface texts are read through it, so a deportment names no localization.
Nothing here touches an entry or a draft.
`LEngine` implements it today, and a settings clerk takes it over when the parts are dismantled.

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

## `IReadOnlyList<string> LEngineLocalizationScan();`

The interface languages the build embeds, so the language box lists no language named in markup.

## `(string LEngineFailureNotice, string? LEngineFailureLabel, string? LEngineFailurePath) LEngineFailureRead(`

The ready notice of a failure: a refusal's reason key, or `unexpected` with `recorded` and the audit file.
One call reads the reason and writes the fault, so Conduct decides nothing about the exception.


## `static string LEngineEnsignFormat(string language, string variety)`

The key a variety's flag is kept under, through the ensign's one key format.
A blank variety has no flag, so its key is empty and finds nothing.
