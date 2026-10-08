# LDraftClerkSentence.cs
Hash: `0e5e7ed4ec851b28`

## `public sealed class LDraftClerkSentence`

The sentence rows of a card.
A row is born blank and named at once, so the form can address it before anything is typed.
The Example inside it is named only when it has something to say.

## `public LDraftClerkSentence(LExampleVault examples, LIdentity identity)`

Holds the shelf a stored Example is picked from and the issuer that names a new row.

## `public LEntryDraft? LSentenceApply(LEntryDraft content, LRequest request)`

Routes every sentence row request to its handler, and answers null for any other request.
An Example or Source request naming card or sentence zero is resolved by the clerk before it reaches here.
The clerk then hands any other request on to the situation chips.

## `public LEntryDraft LSentenceAdd(LEntryDraft content, LRequestSentenceAddition request)`

A new row with a minted id, no Example and an empty frame, at the place asked for.

## `public static LEntryDraft LSentenceRemove(LEntryDraft content, LRequestSentenceRemoval request)`

Drops the row carrying the id from the card, and refuses when the card holds none.

## `public static LEntryDraft LSentenceMove(LEntryDraft content, LRequestSentenceShift request)`

Moves the row carrying the id to the place asked for, and refuses when the card holds none.

## `public LEntryDraft LSentenceSelect(LEntryDraft content, LRequestSentenceExample request)`

Puts the stored Example named under the row, in place of whatever the row quoted.
The row's copy carries the stored id, so commit updates that row rather than making one.
The copy carries the stored Mentions under their positive ids, so a word already linked stays linked.
An id naming no stored Example is refused.

## `public LEntryDraft LExampleChange(LEntryDraft content, long cardId, long sentenceId, Func<LExampleDraft, LExampleDraft> change)`

Applies one change to the Example a row quotes.
A row quoting nothing is offered a blank Example, and the change decides whether it becomes one.
A blank Example the change left blank is dropped again, so an empty text on an empty row is nothing.
One the change wrote into is named here, which is the moment a sentence begins to exist.

## `public static LEntryDraft LSentenceChange(LEntryDraft content, long cardId, long sentenceId, Func<LSentenceDraft, LSentenceDraft> change)`

Applies one change to the row named, and refuses when the card does not hold it.
The Gloss and Mention classes reach a row's Example through here.

## `private static LEntryDraft LSentenceApply(LEntryDraft content, long cardId, Func<IReadOnlyList<LSentenceDraft>, IReadOnlyList<LSentenceDraft>> change)`

Rewrites the row list of one card through the list routine given.
