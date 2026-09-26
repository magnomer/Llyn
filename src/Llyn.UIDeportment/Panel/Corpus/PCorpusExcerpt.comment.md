# PCorpusExcerpt.cs

## `public partial class PCorpus`

The reading side of the Corpus panel, the page one chosen Example is shown on.
The sentence stands at its head and the cited Source under it.
A click on a word asks the engine what it names.

## `private string? PExcerptTextRead(LStateValue value)`

The text a three-state value reads as, or null when it was never written.
An unknown value reads the unknown mark, and whether it is unknown is the converter's to say.

## `private void PExcerptShow(LExample example)`

Paints the read page of an Example the engine announces.
The tally counts the Example the anthology has chosen.

## `private void PExcerptSentenceShow(LExample example)`

Writes the sentence at the head of the page, as a situation's title stands at the head of its page.
A never-written sentence reads the unwritten text in the muted colour, because the head of the page cannot be empty.
The sentence field is the clickable control, and it takes its text through its own property.
Its Mentions are handed over only when the text shown is the stored sentence.
The unknown mark and the unwritten text have no words for a Mention to lie on.

## `private void PExcerptCitationShow(LStateAnchor value)`

Writes the cited Source under its heading, or hides the heading when no citation was ever written.
A source that is not known is itself a Source on the shelf, and reads under its own line.

## `private void PExcerptMentionHandle(object? sender, PMentionArgument e)`

A click on a word of the open Example asks the engine what stands at that offset.
The engine reads the stored sentence itself, so the panel passes the id and the offset and nothing more.
The window decides what the answer opens, as it does for the display cards.
An open draft is confirmed first, because landing on an Entry leaves the corpus panel.
