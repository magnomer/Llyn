# TSituation.cs

## `public sealed class TSituation`

Covers the engine's Situation seam, which mirrors the Tag one: a Situation created, read, rewritten and referenced from both card kinds, the difference between a detach and a remove, and the delete the references refuse.

## Inline notes

### `Assert.Equal(`

The second one asked for position 0, so it takes it and the set is renumbered around it.

### `private static LEntry TSituationEntryCreate(LEngine engine)`

One saved entry with a Meaning card and a Collocation card, neither carrying situations of its own.
