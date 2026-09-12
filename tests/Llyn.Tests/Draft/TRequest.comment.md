# TRequest.cs

## `public sealed class TRequest`

Covers the requests a form sends to edit a held draft, and what the engine answers.

## `public void RequestApply_HeadwordText_PersistsInDraftFile()`

A headword request lands in the file, so a re-read sees it without the returned draft.

## `public void RequestApply_CardAddition_MintsNegativeIdAndRaisesBulletin()`

An added card comes back named with a minted id and numbered one.
One draft bulletin is raised for it, naming the draft.

## `public void RequestApply_CardShift_ReordersAndRenumbersCards()`

A shift lands the card where asked and the whole list reads 1, 2, 3 again.

## `public void RequestApply_CardRemoval_DropsCardAndRenumbersRest()`

A removal drops the one card named and closes the gap in the numbering.

## `public void RequestApply_CardTitle_ChangesOnlyThatCard()`

A field request changes the card it names and leaves its neighbour as it was.

## `public void RequestApply_UnknownCard_Refuses()`

A card the draft does not hold is refused with the card reason.

## `public void RequestApply_ZeroCardId_Refuses()`

Zero names nothing in a draft, so a request carrying it is refused before anything is searched.

## `public void RequestApply_CollocationParent_Refuses()`

A collocation cannot hold a card, so an addition naming one as parent is refused with the nesting reason.

## `public void RequestApply_DraftNotHeld_Refuses()`

A request for a draft this engine does not hold is refused with the draft reason.

## Inline notes

### `private sealed class TRequestObserver : LObserver`

Collects every bulletin raised, so a test can count them and read what they name.
