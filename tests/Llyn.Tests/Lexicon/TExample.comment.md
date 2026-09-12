# TExample.cs

## `public sealed class TExample`

Covers the engine's Example seam.
An Example is created with its renditions.
It is referenced from all three sides that may quote one.
It is rewritten, pointed at a Reference and let go of.
That includes the removal that deletes an Example nothing quotes any more.

## Inline notes

### `Assert.Throws<InvalidOperationException>(() => engine.LEngineExampleDelete(example.LExampleId));`

A delete while something still quotes it is refused.
A detach leaves the row for the side that still does.

### `engine.LEngineExampleUpdate(example.LExampleId, null);`

Clearing the citation leaves both rows standing: only the pointer moved.

### `private static LEntry TExampleEntryCreate(LEngine engine)`

One saved entry with a Meaning card and a Collocation card, neither quoting an Example of its own.

## `public void EntryUpdate_SharedExampleEdited_LeavesOtherCardsSentence()`

An Example two cards quote is pool data, and a card edits only what it owns.
Editing the sentence through one card gives that card a new Example row carrying the new text.
The other card keeps the row it quoted, unchanged, because nobody edited it there.
A card quoting an Example alone still edits it in place, since no one else can see the change.

## `public void ExampleRead_WorkspaceStock_ReturnsUsageCounts()`

The reads the browsing panel stands on.
Every Example comes back with its renditions, including one nothing quotes.
The counts answer for every Example at once, and an unquoted one is simply absent rather than zero.

## `public void UsageRead_ExampleQuotedEverySide_NamesEachSide()`

The itemized usage and the detaching delete.
An Entry, a Meaning and a Collocation each quote on their own terms, and the read says which is which.
The plain delete is still refused while anything quotes it.
The detaching delete drops every reference and the row in one operation.

