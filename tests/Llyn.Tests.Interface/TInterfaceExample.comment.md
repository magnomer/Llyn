# TInterfaceExample.cs
Hash: `f2df04344ad12a53`

## `internal static class TInterfaceExample`

The relays that build the example and sentence records and their drafts.
A record's missing state values default to unspecified, so a test names only what it checks.
Each relay is transparent and carries no test logic of its own.

## `internal static LExample TExampleCreate(long id, string language, LStateValue? text, LStateValue? translation, LStateAnchor? source)`

A present translation becomes the example's single gloss with no language.
An empty or missing translation leaves the example without a gloss.
