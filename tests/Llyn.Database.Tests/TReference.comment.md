# TReference.cs

## `public sealed class TReference`

Covers the engine's bibliographic seams: a Reference created, cited by an Entry and by an Example, credited to Authors in order, and the deletes each side refuses while something still points at the row. Neither a Reference nor an Author is deleted by a citation or a credit going: both are deliberate data, not something typed into a card.

## Inline notes

### `LExample example = engine.LEngineExampleCreate(`

An Example holds one citation, so attaching a second replaces the first rather than adding.

### `Assert.Throws<InvalidOperationException>(() => engine.LEngineAuthorDelete(first.LAuthorId));`

A credited Author is not deletable, and losing the credit does not delete the person.

### `engine.LEngineReferenceDelete(reference.LReferenceId);`

Deleting the Reference takes the credits it owns and leaves the Author it credited.
