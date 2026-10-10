# CCardList.cs
Hash: `672d5101b0d437b9`

## `public sealed class CCardList`

The editor's two card lists share gates for adding, removing, moving and folding whole cards.
It is split from `CCard` by role, and its members keep the `CCard` base.
It keeps no state of its own, so the editor builds it once over its desk and ports.
Every draft rule sits in `LDraftClerkCard`, reached through one `LQuillCard` call per gate.
The fold gate goes to the card port instead, since a fold is view state and no edit.
Both move gates first turn their place into the engine's index through `CFolio.CFolioPlaceRead`.

## `private readonly CDesk _cCardListDesk;`

The editor's desk, whose held tenure the draft gates edit and whose stored id the fold gate names.

## `private readonly LCardPort _cCardListPort;`

The card port a fold is saved and deleted through.

## `private readonly LSettingsPort _cCardListSettings;`

The settings port a refused fold words its notice through.

## `private readonly CEnvoy _cCardListEnvoy;`

The editor's envoy, through which a refused fold shows its notice.

## `internal CCardList(CDesk desk, LCardPort cards, LSettingsPort settings, CEnvoy envoy)`

Only the editor builds it, over its own desk and the ports it was handed.

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

## `public bool CCardFoldToggle(long cardId, bool folded)`

The gate for a card's chevron in the editor, which folds or unfolds that card.
The card id is the card draft's own `CCardDraftId`, handed back unread.
It names the held entry by its stored id, and a draft never stored sends nothing.
An unsaved card writes nothing, since the clerk below refuses its id.
It goes straight to the card port, so it never dirties the draft, makes a revision or enters undo.
The port raises the fold bulletin, and `CEntryDraftChanged` then hands every card its new fold.
The reading view on the same entry repaints too.
True means the port returned without throwing, not that a stored row changed.
A draft never stored and a throwing port call both answer false.
A nonpositive card id still answers true when the clerk ignores it without throwing.
The caller puts its chevron back on false, since no bulletin will repaint it.
The verdict comes from the one call alone, with no read-back.
A refused write shows `Fold.SaveFailed` every time, since the user acted.

## `public void CCardMove(long cardId, int place)`

The move gate for a drag, which hands the place its geometry found.
A place is the index of a card in the list `CEntryDraftChanged` last handed out.
That list is in the order `CFolio` sets, so the gate maps the place through `CFolio.CFolioPlaceRead`.
A place outside the handed list sends nothing.
The quill asks the card clerk to judge the engine index before requesting a move.
A stale card or an unchanged place sends nothing.
The view hands every place it finds and keeps no rule of its own.

## `public void CCardMove(long cardId, string ordinal)`

The move gate for the position badge, which hands the raw typed text.
`CFolio.CFolioOrdinalRead` reads the text as a place in the handed list, clamped to its ends.
The place then goes through the drag gate, so the typed number counts in the shown order.
The engine's own ordinal rule counts its raw list, which may not be in position order.
Text that is no number, or a desk with no draft, sends nothing.
The two overloads are one action heard in two raw forms.
One signature would make the view parse text or format a number.
