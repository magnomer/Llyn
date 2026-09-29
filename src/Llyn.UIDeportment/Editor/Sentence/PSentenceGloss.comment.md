# PSentenceGloss.cs

## `internal sealed partial class PSentence`

The Gloss rows a card's sentence row shows under its sentence.

## `public ObservableCollection<PGloss> PSentenceGloss { get; }`

The rows in the order the Example keeps them.

## `internal Action<PGloss, string>? PSentenceGlossNotice { get; set; }`

Where a language picked in one of the rows goes, with the raw language.
The card sets it when it builds the row, so the pick reaches the editor with its sentence.

## `private void PSentenceGlossShow(IReadOnlyList<CGlossDraft> drafts)`

Redraws the rows from the drafts, keeping the rows whose ids survive.

## `private PGloss PSentenceGlossCreate(CGlossDraft draft)`

One row, subscribed so its language pick reaches the card through the notice.
