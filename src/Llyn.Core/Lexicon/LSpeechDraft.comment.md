# LSpeechDraft.cs

## `public sealed record LSpeechDraft(`

One part of speech as the input form carries it.

A part of speech is either a value the language pack defines or a name the user typed.
The draft keeps the two apart so a custom name never imports as a lookup.

**Parameters**

- `LSpeechDraftValue` — Stable id of the language-pack value, or null when the user typed a name.
- `LSpeechDraftCustom` — Name the user typed, or null when a language-pack value stands instead.
- `LSpeechDraftName` — Name to show, resolved from the value id where the pack names it.
  It is display text and is never stored.

## `public string LSpeechDraftName { get; init; }`

Falls back to whichever of the two the draft carries when no display name was resolved.

## `public static LSpeechDraft LSpeechDraftCreate(string name)`

Builds the draft the input form makes from a typed name.
The engine decides at the write whether a preset names it.

## `public bool LSpeechDraftEmpty`

True when the draft names neither a value nor a custom name.
