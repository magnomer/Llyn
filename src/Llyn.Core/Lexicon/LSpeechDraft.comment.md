# LSpeechDraft.cs

## `public sealed record LSpeechDraft(`

One part of speech as the input form carries it.

A part of speech is either a value row the language declares or a name the user typed.
The draft keeps the two apart so a custom name never imports as a link.

**Parameters**

- `LSpeechDraftValue` — Linked `speech_value` row id, or `0` when the user typed a name.
- `LSpeechDraftCustom` — Name the user typed, or null when a value row stands instead.
- `LSpeechDraftName` — Name to show, read from the value row where one is linked.
  It is display text and is never stored.

## `public string LSpeechDraftName { get; init; }`

Falls back to the custom text when no display name was resolved.

## `public static LSpeechDraft LSpeechDraftCreate(string name)`

Builds the draft the input form makes from a typed name.
The engine decides at the write whether a value row names it.

## `public static LSpeechDraft LSpeechDraftCreate(long valueId, string name)`

Builds the draft the input form makes from a value row it already knows.

## `public bool LSpeechDraftEmpty`

True when the draft links no value and carries no custom name.
