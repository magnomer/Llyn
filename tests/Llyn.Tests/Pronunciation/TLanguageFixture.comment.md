# TLanguageFixture.cs

## `internal sealed class TLanguageFixture : IDisposable`

A throwaway language pack written beside the shipped ones, under a name no other test uses.
The engine loads it by that name exactly as it loads a shipped pack.
Disposing removes the folder again.

## `internal static LLanguage TLanguageFixtureLoad(string json)`

Writes the pack, loads it through the relay and removes it again.
A test that only reads the loaded pack needs no handle on the folder.

## `internal void TLanguageFixtureSave(string file, string json)`

Writes a second file beside the pack's source.json, for a pack that names a file of its own.
