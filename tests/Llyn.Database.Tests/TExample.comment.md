# TExample.cs

## `public sealed class TExample`

Covers the engine's Example seam: an Example created with its translations, referenced from all three sides that may quote one, rewritten, pointed at a Reference and let go of — including the removal that deletes an Example nothing quotes any more.

## Inline notes

### `Assert.Throws<InvalidOperationException>(() => engine.LEngineExampleDelete(example.LExampleId));`

A delete while something still quotes it is refused; a detach leaves the row for the side that still does.

### `engine.LEngineExampleUpdate(example.LExampleId, null);`

Clearing the citation leaves both rows standing: only the pointer moved.

### `private static LEntry TExampleEntryCreate(LEngine engine)`

One saved entry with a Meaning card and a Collocation card, neither quoting an Example of its own.
