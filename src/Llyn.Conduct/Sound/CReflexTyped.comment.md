# CReflexTyped.cs
Hash: `2cb16d864cfce53d`

## `public sealed record CReflexTyped(CReflexField CReflexTypedField, string CReflexTypedText, IReadOnlyList<CReflexHead> CReflexTypedHeads)`

The answer to one typed reflex cell, as `CTimbre.CTimbreReflexSet` gives it.
A driver writes its cell from this answer alone, never from what the user typed.

**Parameters**

- `CReflexTypedField`: the cell the answer is for.
- `CReflexTypedText`: the text the cell now holds.
  It is the typed text when the gate took it, else the text the draft holds.
- `CReflexTypedHeads`: every row's lead while a language is typed, else empty.

## `public string CReflexTypedKey`

The localization key the answered text is labelled under, by the rule of `CReflex.CReflexLanguageKey`.
A driver relabels a typed language or kind from it, so the label never names the previous text.
