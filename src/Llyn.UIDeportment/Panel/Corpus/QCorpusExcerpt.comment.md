# QCorpusExcerpt.cs

## `internal sealed partial class QCorpus`

The reading side of the Corpus panel, the page one chosen Example is shown on.
The sentence stands at its head and the cited Source under it.
A click on a word asks the engine what it names.

## `private static string? QExcerptTextRead(CStateValue value)`

The text a three-state value reads as, or null when it was never written.

## `private void QExcerptShow(CExample example)`

Paints the read page of an Example the engine announces.
The tally counts the Example the anthology has chosen.

## `private void QExcerptSentenceShow(CExample example)`

Writes the sentence at the head of the page, as a situation's title stands at the head of its page.
A never-written sentence reads the unwritten text in the muted colour, because the head of the page cannot be empty.
Its Mentions reach the mention text as they are stored.
The controller's map leaves them out unless the text reads soundly, so the excerpt takes them as they come.

## `private void QExcerptCitationShow(long? value)`

Writes the cited Source under its heading, or hides the heading when no citation was ever written.

## `private void QExcerptMentionHandle(object? sender, PMentionArgument e)`

A click on a word of the open Example asks the engine what stands at that offset.
An open draft is confirmed first, because landing on an Entry leaves the corpus panel.
The chosen Example is passed inline, so no driver local carries an engine answer.
A failure to find is shown by the window, as before.

## `private void QExcerptMentionShow(CMentionResult? result)`

Hands what the click found to the window, which decides what it opens.
Nothing is chosen when the result is null, so nothing opens.
