# List

List holds the meaning and collocation card lists of the editor.
Both lists share one card renderer and one set of handlers.
Card identity is independent of list position.
Gestures reach Conduct gates, and ready draft values drive presentation.

## `QCardGhost`

The ghost is a dimmed copy of the dragged card painted above the list.
The card itself stays on screen and keeps its place until the drag moves it.
The ghost is hit-testable by nothing, so the list underneath still hears the pointer.
Its size and left edge are fixed at the start, so it moves only up and down.

### Inline notes

### QCardGhost.cs: `_qCardGhostFace = new VisualBrush(card) { Stretch = Stretch.None }`

A brush of the live card rather than a snapshot keeps the card's own look.

## `QBond`

This is the driver of a collocation card's Meanings field, split by role.
Each Observe hears one event and calls at most one card-field gate, and each Refine only changes the look.
Attaching, detaching, ordering and the home rule live below Conduct.
`QCard` owns one instance, which keeps chip work out of `QCard`.

### `QBondApply`

It serves two kinds of container, a card and a chip.
A meaning card has no `PCardBond`, so it gets nothing.
Handlers are removed before they are added, since containers are filled again on every change.

### `QBondRefine`

The first chip is the home, and a new link arrives last.
A chip with the same meaning id, path and name keeps its row, so an unchanged chip is not rebuilt.

### `QBondMenuRefine`

An empty list shuts the menu, so opening, picking and Escape all paint through this one method.
The popup and its list are template parts, so each card owns its own menu.
The anchor is set here, because markup holds no binding.

### `QBondRowRefine`

The indent goes on the path, so a sub-meaning's whole line steps in by depth.

### `QBondChooserObserve`

The answer lists every meaning of the entry, drafts included, as `QBondItem` rows.
So no menu row holds a Conduct meaning.

### `QBondMenuObserve`

The menu shuts first, then the picked meaning is linked.
The card comes from the picked row's own card id, never from a remembered field or the menu's anchor.

### `QBondKeyObserve`

Keyboard focus stays on the chooser while the popup shows, so Escape shuts it there.
An outside click needs no handler, because the popup does not stay open.

### `QBondEraserObserve`

The chip carries both its card's id and its meaning's id, so no control is searched for the card.

## `QBondItem`

This is one linked meaning, as a chip or as a row of the chooser menu.
It is the Deportment item the surface holds, so no control ever holds a Conduct meaning.
Every value is fixed at construction, so a chip never changes which link it names.
The card's id rides along, so the chip's eraser names its card.
The indent step matches the mention menu's, so both pickers show the meaning tree alike.

## `QCard`

This is the shared renderer and handler set of both card lists.
Field drivers and sentence-menu collections are shared by both card kinds.
The badge driver needs no list, since position requests go to Conduct by card id.

### `QCardApply`

Handlers are removed before reattachment, avoiding duplicate subscriptions during refills.
The changed-property name limits field text replacement.

### `QCardRowRefine`

Unrelated property changes leave field text alone, preserving another field's caret.
The title box stays visible when a folded untitled card shows its peek, preserving editing focus.
Hints, folded peeks and fold shape follow every refill.

### `QCardHingeObserve`

The verdict reaches `QLook.QLookCheckedRefine`, which restores refused clicks.
Accepted folds repaint through ready draft notifications.

### `QCardTitleObserve`

Only keyboard-focused fields report raw text to a gate.
So ordinary unfocused rendering echoes do not become edits.
The expression and definition fields follow the same rule.

### `QCardRemoveObserve`

Removal sends identity rather than editing the presentation collection.
The identity is the card's collocation flag and id.
Conduct decides whether removal is allowed.

### `QCardRefine`

Each draft claims one existing card by id or receives a new presentation item.
The caller says whether the list is the collocation list, and a new card is built with that flag.
The same flag reads the card's caret anchors.
The supplied draft order is authoritative.
Reconciliation keeps its longest leading sequence found in the existing order, removing others and appending the remainder.
Only that kept sequence retains controls and caret.
Re-added items retain their objects but may lose focus when their controls rebuild.
The caller chooses each new card's peek wording, which remains derived rather than copied.

### `QCardDraftRefine`

The supplied position is independent of loop position.
The four chip fields take their anchors from one caret gate read, so every refill reseats each entry.
Fold verdicts arrive last, allowing an in-place fold repaint after other fields are current.

## `QCardDrag`

This class drags a card to another place in its list by its header.
A ghost follows the pointer while the card stays on screen.
The list rearranges as the ghost's leading edge crosses the middle of a neighbour.
So the order is already the new order when the button comes up.
The engine is told each move as it happens, and hands back the numbering.
The list order is the stored order, so no drag is left for the store to discover.
Drag state is the gesture's alone, so it stays in the deportment.
The lists, not the cards, hear the move and the release, since the capture is taken on the list.

### `QCardDragRefine`

The list is the one whose items hold the card, since each list shows exactly its cards.

### Inline notes

### QCardDrag.cs: `private QCardGesture? _qCardDragGesture`

A new press replaces it, so no state of an earlier drag reaches the next.

### QCardDrag.cs: `if (host is null || host.Items.Count <= 1)`

A list of one has nowhere else to put its card, so no ghost begins on it.
This only decides whether the drag is shown.

### QCardDrag.cs: `host.CaptureMouse()`

The card slides out from under the pointer as soon as the order changes.
The list has to keep hearing the pointer until the button comes back up.

### QCardDrag.cs: `if (!gesture.QCardGestureShow(e))`

A ghost that could not be painted ends the drag, so nothing is left to move.

### QCardDrag.cs: `int target = gesture.QCardGestureResolve(e)`

The drag distance is far shorter than half a card, so a click never finds a new place.
Whether the place is a move at all is the move gate's answer, not the view's.

### QCardDrag.cs: `private void QCardDragReset(object sender, MouseEventArgs e)`

It is heard both when the button comes up and when capture is lost.
Capture can be taken away by another window or a menu without the button coming up.
A ghost left painted over the list would outlive its drag.

### QCardDrag.cs: `_cCardList.CCardMove(collocated, gesture.QCardGestureId, target)`

The place goes to the one move gate the position badge also uses.
The flag is read off the dragged item, which is a collocation placard or not.
It is sent on every pointer move, even when the place is unchanged.
The gate sends nothing for an unchanged place or a card the draft lacks.
The list is moved by the answer, because moving it first would show an order the engine may refuse.

### QCardDrag.cs: `_qCardDragGesture = null`

It is cleared before the capture is released.
Letting go raises the lost-capture notice, which must find a drag that is already over.

## `QCardFold`

Writing and reading cards share one fold painter, so their hinge and body shapes cannot diverge.
The caller supplies both verdicts, so the painter needs no card row or persistence dependency.
Stored state independently controls hinge visibility.
Unfolding clears local header overrides, so the style supplies its normal shape.
Missing named parts do not prevent other parts from being painted.

## `QCardGesture`

This is one press of a card drag, from the press on its header until the drag ends.
`QCardDrag` makes one per press and drops it at the end, so no drag state outlives its press.
It takes only what it uses of the card, so it names no card control.
Grip and height are read at once, since the list rearranges under the drag.
The item is a handle on the row shown, never a copy of the list.
The id is read once at the press, since it never changes.

### `QCardGestureShow`

A press is not a drag, so no ghost is painted until the pointer has travelled far enough.
It answers false when a ghost cannot be painted, which ends the drag.

### `QCardGestureResolve`

The card is judged by its own edges and never by the pointer.
Where inside the header it was taken hold of would otherwise decide how far it travels.
Its leading edge crosses a neighbour, the top going up and the bottom going down.
So either direction swaps at the same half-card, whatever the grip was.

### `QCardGestureClear`

It takes the ghost off the list and lets the capture go.

### Inline notes

### QCardGesture.cs: `private readonly double _qCardGestureGrab`

The ghost is held at the same distance from the pointer, so the card does not jump under the hand.

### QCardGesture.cs: `private readonly double _qCardGestureHeight`

The order is decided from where the ghost is, not from where the pointer is.

### QCardGesture.cs: `if (index < current && top < middle)`

Going up, the target is the first card whose middle the ghost's top has risen above.
Going down, it is the last card whose middle the bottom has fallen past.
So one long drag passes every card it crosses.

## `QCardPosition`

This is the driver of a card's position badge, which reorders a card by its typed number.
The list gate decides whether a badge may open, since a list of one offers none.
The typed number goes to the list gate, and the engine owns the numbering.
Open, close and settle name the card by its collocation flag and id.
Escape tells the gate the badge closed, and settling tells it the typed text.
Each handler is removed before it is added, so a refill never subscribes twice.

### Inline notes

### QCardPosition.cs: `private void QCardPositionRefine(object sender, MouseButtonEventArgs e)`

A double click opens the badge and stops that click from starting a drag.
A list of one shows no editable badge, as it shows no drag ghost.
The box is focused after the layout runs, because it is not hit tested until then.

### QCardPosition.cs: `private void QCardPositionObserve(object sender, RoutedEventArgs e)`

The typed text stays in the box as pure medium, so the card's number has one writer.
One handler hears Enter and leaving, since both commit the same text.
The gate's settle verdict decides whether Enter is handled, and the badge hides either way.
A badge the gate no longer holds open settles nothing.

## `QCollocation`

This is the driver for the collocation list and the button that adds a collocation card.
The cards themselves answer to `QCard`.

### Inline notes

### QCollocation.cs: `private void QCollocationAddObserve(object sender, RoutedEventArgs e)`

The card arrives through the bulletin, already named and placed, so the form never guesses an id or a place.

## `QMeaning`

This is the driver for the meaning list and the button that adds a meaning card.
It follows `QCollocation` in everything.

### Inline notes

### QMeaning.cs: `private void QMeaningAddObserve(object sender, RoutedEventArgs e)`

The card arrives through the bulletin, already named and placed, so the form never guesses an id or a place.

## Stamp

Commit: `8c403d1df768`
- `QBond.cs` `884c7c93369269b3`
- `QBondItem.cs` `91b81f9342fbd78b`
- `QCard.cs` `87fdd0e31ff54533`
- `QCardDrag.cs` `ea423f6c8d2c949f`
- `QCardFold.cs` `e15e8aeeda7c7de4`
- `QCardGesture.cs` `aadadf014410a661`
- `QCardGhost.cs` `a8d2e5cedb6f2ce5`
- `QCardPosition.cs` `4c0868252612d5ba`
- `QCollocation.cs` `5c35cece54f82067`
- `QMeaning.cs` `2a619d7b05708280`
