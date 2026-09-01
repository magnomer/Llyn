# TTag.cs

## `public sealed class TTag`

Covers the engine's Tag seam: a Tag created, read, renamed and referenced from both card kinds through `LEngine`, the difference between detaching a reference and removing one, and the refusals that meet a delete the references forbid and a side a Tag never hangs from.

## Inline notes

### `engine.LEngineTagAttach(senseId, rare.LTagId, 0, LOwner.LOwnerSense);`

One row, referenced from both sides, each side holding its own order over what it references.

### `engine.LEngineTagUpdate(formal with { LTagText = "formal register" });`

A rename reaches every reference at once: a reference points at the row, never at its words.

### `engine.LEngineTagDetach(senseId, tag.LTagId, LOwner.LOwnerSense);`

Detaching is what editing a card does: the reference goes, the row other cards use stays.

### `engine.LEngineTagAttach(senseId, tag.LTagId, 0, LOwner.LOwnerSense);`

Removing is the other intention, and it still leaves a Tag another card references.

### `engine.LEngineTagRemove(collocationId, tag.LTagId, LOwner.LOwnerCollocation);`

The last reference going takes the row with it, so no Tag is left that nothing can reach.

### `Assert.Throws<ArgumentOutOfRangeException>(() =>`

An Entry references Examples but never Tags, so there is no table to attach this to.

### `private static LEntry TTagEntryCreate(LEngine engine)`

One saved entry with a Meaning card and a Collocation card, neither carrying tags of its own, so every Tag in these tests is one the seam wrote.
