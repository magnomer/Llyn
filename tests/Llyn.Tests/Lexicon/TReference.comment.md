# TReference.cs

## `public sealed class TReference`

Covers the engine's bibliographic seams.
A Reference is created, cited by an Entry and by an Example.
It is credited to Authors in order.
Each side refuses a delete while something still points at the row.
Neither a Reference nor an Author is deleted by a citation or a credit going.
Both are deliberate data, not something typed into a card.

## Inline notes

### `LExample example = engine.LEngineExampleCreate(`

An Example holds one citation, so attaching a second replaces the first rather than adding.

### `Assert.Throws<InvalidOperationException>(() => engine.LEngineAuthorDelete(first.LAuthorId));`

A credited Author is not deletable, and losing the credit does not delete the person.

### `engine.LEngineReferenceDelete(reference.LReferenceId);`

Deleting the Reference takes the credits it owns and leaves the Author it credited.
