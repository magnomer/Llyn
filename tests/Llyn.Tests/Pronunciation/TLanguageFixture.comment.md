# TLanguageFixture.cs

## `internal sealed class TLanguageFixture : IDisposable`

A throwaway language pack written beside the shipped ones, under a name no other test uses.
The engine loads it by that name exactly as it loads a shipped pack.
Disposing removes the folder again.
