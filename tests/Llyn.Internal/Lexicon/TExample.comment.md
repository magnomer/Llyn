# TExample.cs

## `public sealed class TExample`

Covers Examples as the entry editor quotes them.
An Example is created with its renditions and quoted from both card sides by a sentence citing it.
What a card quotes is read back through the entry's load, as the editor reads it.
It is rewritten through a card and pointed at a Reference through a card.

## `public void ExampleRead_FortyThousandExamples_ReadsEveryGlossAndMention()`

The whole-table read fills every Example's Glosses and Mentions in one statement each.
Past 32 766 Examples that statement can no longer bind one parameter per id.
So the ids travel as one JSON value.
A corpus that deep must still open.

## `public void EntryUpdate_SharedExampleEdited_LeavesOtherCardsSentence()`

An Example two cards quote is pool data, and a card edits only what it owns.
Editing the sentence through one card gives that card a new Example row carrying the new text.
The other card keeps the row it quoted, unchanged, because nobody edited it there.
A card quoting an Example alone still edits it in place, since no one else can see the change.

## `public void ExampleRead_WorkspaceStock_ReturnsUsageCounts()`

The reads the browsing panel stands on.
Every Example comes back with its renditions, including one nothing quotes.
The counts answer for every Example at once, and an unquoted one is simply absent rather than zero.

## `public void UsageRead_ExampleQuotedBothCardSides_NamesEachSide()`

The itemized usage and the detaching delete.
A Meaning and a Collocation each quote on their own terms, and the read says which is which.
The plain delete is still refused while anything quotes it.
The detaching delete drops every reference and the row in one operation.

## `private static LEntry TExampleEntryCreate(LEngine engine, IReadOnlyList<LSentenceDraft> meaningSentences, IReadOnlyList<LSentenceDraft> collocationSentences)`

One saved entry with a Meaning card and a Collocation card, each carrying the sentences given.

## `private static LSentenceDraft TExampleCiteCreate(LExample example)`

A sentence citing a stored Example by id, with the text and source it already holds.
Anything else would edit the Example rather than quote it.

## `private static LCardDraft TExampleCardRead(LEngine engine, long entryId, bool collocation)`

The first Meaning or Collocation card of the entry as its load returns it.
