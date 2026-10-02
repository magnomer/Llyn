# CCardList.cs
Hash: `3994cad19350a223`

## `public sealed class CCardList`

The gates of the editor's two card lists: adding, removing and moving whole cards.
It is split from `CCard` by role, and its members keep the `CCard` base.
It keeps no state, so the editor builds it fresh over its desk.
Every rule sits in `LDraftClerkCard`, reached through one `LQuillCard` call per gate.

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
The card clerk judges the place, so a stale card or an unchanged place sends nothing.
The view hands every place it finds and keeps no rule of its own.

## `public void CCardMove(long cardId, string ordinal)`

The move gate for the position badge, which hands the raw typed text.
The parse, the clamp and the unchanged place are the card clerk's.
The two overloads are one action heard in two raw forms.
One signature would make the view parse text or format a number.
