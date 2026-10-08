# CCardList.cs
Hash: `836537bdfa93b55b`

## `public sealed class CCardList`

The gates of the editor's two card lists: adding, removing and moving whole cards.
It is split from `CCard` by role, and its members keep the `CCard` base.
It keeps no state, so the editor builds it fresh over its desk.
Every rule sits in `LDraftClerkCard`, reached through one `LQuillCard` call per gate.
Both move gates first turn their place into the engine's index through `CFolio.CFolioPlaceRead`.

## `private LQuillCard? CCardListQuill`

The card edits over the held draft, or none while the desk is filling or holds no draft.
So a gate pressed during a fill sends nothing, as every other editor gate does.

## `public void CCardMeaningAdd()`

The gate for the meaning list's add button.
Where the new card lands is the card clerk's default, not a count the view hands in.

## `public void CCardCollocationAdd()`

The gate for the collocation list's add button, placed as a new meaning is.

## `public void CCardRemove(long cardId)`

The gate for a card's eraser.
A card alone in its list is kept by the clerk's rule, so the view sends every press.

## `public void CCardMove(long cardId, int place)`

The move gate for a drag, which hands the place its geometry found.
A place is the index of a card in the list `CEntryDraftChanged` last handed out.
That list is in the order `CFolio` sets, so the gate maps the place through `CFolio.CFolioPlaceRead`.
A place outside the handed list sends nothing.
The card clerk judges the engine index, so a stale card or an unchanged place sends nothing.
The view hands every place it finds and keeps no rule of its own.

## `public void CCardMove(long cardId, string ordinal)`

The move gate for the position badge, which hands the raw typed text.
`CFolio.CFolioOrdinalRead` reads the text as a place in the handed list, clamped to its ends.
The place then goes through the drag gate, so the typed number counts in the shown order.
The engine's own ordinal rule counts its raw list, which may not be in position order.
Text that is no number, or a desk with no draft, sends nothing.
The two overloads are one action heard in two raw forms.
One signature would make the view parse text or format a number.
