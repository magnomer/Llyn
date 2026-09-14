# LMarkupExample.cs

## `public sealed record LMarkupExample(`

One example sentence as a markup file carries it.
Its reference travels by value rather than by id, and its mentions name their targets by natural key.

**Parameters**

- `LMarkupExampleText` — The sentence and what is known about it.
- `LMarkupExampleLanguage` — The language code of the sentence.
- `LMarkupExampleGloss` — Gloss drafts with id zero, in file order.
- `LMarkupExampleMention` — Mentions in file order.
- `LMarkupExampleReference` — The cited reference, and `null` when the example cites none.

## `public bool Equals(LMarkupExample? other)`

Row-by-row equality in order, the reference compared by value.

## `public override int GetHashCode()`

A hash over the text, language, reference and row counts.
