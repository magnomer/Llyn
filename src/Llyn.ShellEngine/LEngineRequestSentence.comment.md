# LEngineRequestSentence.cs

## `public sealed partial class LEngine`

The sentence rows of a card.
A row is born blank and named at once, so the form can address it before anything is typed.
The Example inside it is named only when it has something to say.

## `private LEntryDraft LEngineSentenceAdd(LEntryDraft content, LRequestSentenceAddition request)`

A new row with a minted id, no Example and an empty frame, at the place asked for.

## `private LEntryDraft LEngineSentenceSelect(LEntryDraft content, LRequestSentenceExample request)`

Puts the stored Example named under the row, in place of whatever the row quoted.
The row's copy carries the stored id, so commit updates that row rather than making one.
An id naming no stored Example is refused.

## `private LEntryDraft LEngineExampleChange(`

Applies one change to the Example a row quotes.
A row quoting nothing is offered a blank Example, and the change decides whether it becomes one.
A blank Example the change left blank is dropped again, so an empty text on an empty row is nothing.
One the change wrote into is named here, which is the moment a sentence begins to exist.

## `private static LEntryDraft LEngineSentenceChange(`

Applies one change to the row named, and refuses when the card does not hold it.

## `private static LEntryDraft LEngineSentenceApply(`

Rewrites the row list of one card through the list routine given.
