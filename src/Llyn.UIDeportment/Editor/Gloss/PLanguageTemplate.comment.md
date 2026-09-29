# PLanguageTemplate.cs

## `public class PLanguageTemplate : ResourceDictionary`

This dictionary draws a card language field while leaving language state with the editor.
The editor's fill subscribes its own Observe on the realized part, so the dictionary holds no handler.

## `internal PLanguageTemplate()`

Merges the markup the Veneer holds, since the dictionary carries no class of its own there.
