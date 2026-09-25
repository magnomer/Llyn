# TReference.cs

## `public sealed class TReference`

Covers the engine's bibliographic seams.
A Reference is created, cited by an Example and credited to Authors in order.
Each side refuses a plain delete while something still points at the row.
The detaching delete the catalog runs lets go of every pointer and the row together.
Neither a Reference nor an Author is deleted by a citation or a credit going.
Both are deliberate data, not something typed into a card.

## Inline notes

### `Assert.NotNull(engine.TEngineExampleRead(example.LExampleId));`

The Example that cited the Reference stays, only its pointer goes.

### `Assert.Throws<InvalidOperationException>(() => engine.TEngineAuthorDelete(first.LAuthorId, false));`

A credited Author is not deletable until the delete detaches its credits.

### `engine.TEngineReferenceDelete(reference.LReferenceId, true);`

Deleting the Reference takes the credits it owns and leaves the Author it credited.
