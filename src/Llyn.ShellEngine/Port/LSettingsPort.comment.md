# LSettingsPort.cs

## `public interface LSettingsPort`

The slice of the engine the window and the settings panel see.
It reads and saves the settings, the workspace path, the workspace state and the interface language.
It also reads the rescue report, records an audit line, words a refusal, and loads fonts and flags.
The interface texts are read through it, so a deportment names no localization.
Nothing here touches an entry or a draft.
`LEngine` implements it today, and a settings clerk takes it over when the parts are dismantled.

## `void LEngineWorkspaceChange(string path);`

Moves the engine onto another workspace folder and records it, so Conduct changes the workspace without the engine itself.

## `IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language);`

The interface strings of one language, parsed and ready for the resource dictionary.

## `IReadOnlyList<string> LEngineLocalizationScan();`

The interface languages the build embeds, so the language box lists no language named in markup.

## `string? LEngineAuditRecord(Exception exception);`

Writes the failure to the audit log and returns the path written, or null when the log is unreachable.

## `string LEngineGlossRead();`

The language a new translation of an Example starts in.
It is the gloss language of the settings when its pack is loaded, else the first loaded language.
