# TMarkupImport.cs

## `public sealed class TMarkupImport`

Covers the seam between the markup reader and the store.
A document goes in as text and comes out as entries that load back as the file wrote them.
The sources it declared become reference rows its citations name.
A file with anything broken in it adds nothing at all.

The parse itself is covered in `TMarkupSyntax.cs`.
What is tested here is only what needs a workspace.
That is the rows, the shared source, and the atomicity.

## Inline notes

### `private const string TMarkupSample =`

The sample from section 8 of `docs/Format-LlynMarkup.md`, less the `<audio>` and `<synonym>` tags.
An audio path is stored relative to the workspace.
A card's synonym text is deliberately dropped by the save.
So neither reads back, and asserting on them would test the sample rather than the import.

### `IReadOnlyList<LEntry> imported = engine.LEngineMarkupImport(TMarkupSample);`

One call takes the whole file.
The entries come back in the order the document wrote them.

### `Assert.Equal(LState.LStateUnknown, oed.LReferenceProgram.LStateValueState);`

An empty `<program></program>` survives the round trip as *unknown*, distinct from the tags the source never wrote.

### `Assert.Equal(`

The author the source credited is its own row, attached to the reference in the order the block wrote it.

### `Assert.Equal(`

The second meaning holds the three states side by side.
There is a written tag and an empty one.
There is an example citing nothing and one citing a source that cannot be read.

### `Assert.Equal(LState.LStateUnknown, field.LReferenceAuthorState);`

A lone empty `<author></author>` credits somebody unreadable.
The state says so and no author row is written, because there is no name to write.

### `LReference oed = Assert.Single(engine.LEngineReferenceRead());`

Two examples citing one `id` share one reference row rather than each declaring its own.

### `Assert.Throws<FormatException>(() => engine.LEngineMarkupImport(`

The second entry cites a source no entry declares.
The first entry is perfectly good and is still not stored.
An import is one unit of work, so a file either imports whole or not at all.
