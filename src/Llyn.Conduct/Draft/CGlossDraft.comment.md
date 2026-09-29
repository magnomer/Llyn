# CGlossDraft.cs

## `public sealed record CGlossDraft(`

One gloss of an example, as its gloss row shows it.

**Parameters**

- `CGlossDraftId`: the stored gloss, zero for a fresh one.
- `CGlossDraftLanguage`: the gloss's language.
- `CGlossDraftText`: the gloss's text.
- `CGlossDraftNamed`: the engine's verdict that a language is chosen.

## `public string? CGlossDraftHint`

The key a row shows in place of a language while none is chosen, or null once one is.
Conduct chooses the key, so the row only looks it up.
