# TReference.cs
Hash: `0ac9eace2d168f67`

## `public sealed class TReference`

Covers the engine's bibliographic seams.
A Reference is created, cited by an Example and credited to Authors in order.
Each side refuses a plain delete while something still points at the row.
The detaching delete the catalog runs lets go of every pointer and the row together.
Neither a Reference nor an Author is deleted by a citation or a credit going.
Both are deliberate data, not something typed into a card.
Usage reads name every card and Example that cites a Reference or sits under a credited Author.
A Reference whose stored author state word is broken reads as unreadable and refuses commit until swept.

## Inline notes

### `Assert.NotNull(engine.TEngineExampleRead(example.LExampleId));`

The Example that cited the Reference stays, only its pointer goes.

### `Assert.Throws<InvalidOperationException>(() => engine.TEngineAuthorDelete(first.LAuthorId, false));`

A credited Author is not deletable until the delete detaches its credits.

### `engine.TEngineReferenceDelete(reference.LReferenceId, true);`

The detaching delete drops the Reference and its links, and leaves every Example and Author it touched.
