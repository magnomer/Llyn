# Card

This folder holds the tests of the card editor driver parts.
They cover chip caret keys, chip seat, card fold hinges, ordinal badge and row matcher.
They also cover the editor walk's card lists and the reading view's word cursor.

## `TBerthSeat`

The panel of a chip field seats the typing entry among the chips.
The entry stands right before the chip the caret is anchored to.
No anchor, or an anchor no chip carries, seats the entry at the end.
The chips keep their order around the entry.

## `TCardHinge`

The writing card's fold hinge is covered as the row painter and the click handler see it.
`TLecternHinge` borrows the row builders here for the reading card.
The rows are bare templates that carry only the parts the painters touch, under their real names.
The card drafts carry no meaning links.
A row takes its fold from the editor walk's step.
The step has no fold function when the card is not stored.
So the hinge cases build steps or take them from a real walk.
Each list holds the two fold keys the theme defines, so a folded header resolves them.
The click cases reach the real fold gates over a real workspace.
So the store is read back rather than a double asked.
A folded card hides its body, checks its hinge and closes its header.
A folded card without a title shows the peek and keeps its title box shown and focusable.
So a writer can type a title without unfolding the card first.
A card that is not stored hides its hinge and keeps its body.
A hinge click on a stored card hands the card's kind, its id and the checked state on.
They go to the editor's list gate.
The stored folds are read back as kind and id pairs, and the writing card folds as a meaning.
When no stored entry is held the gate refuses the write, and the hinge returns to unchecked.
No refresh would repaint it, so the hinge must put itself back.
A refused unfold puts the hinge back to checked the same way.
Two clicks before the list rebuilds leave the card unfolded, so a click never reads a stale fold.
An unfolded hinge draws the expand icon turned half a turn, which the app reads as open.
A folded hinge leaves the same icon unturned, so it reads as closed and able to open.

### `TCardHingeBlank`, `TCardHingeTitle` and `TCardHingeMeaning`

These shared wordings keep the cases distinct.
The blank one is muted, so folded cards show their peek.
The meaning one provides the peek both painting suites expect.

### `TCardHingeCreate`

The hinge is a plain `ToggleButton`, as in the real templates, so a driver's type test finds it.
A template root is not found by name, so the hinge stands one level down.

### `TCardLookPrepare`

It gives the application the resources the look sheet reads when it first loads.
It points the icon root at the icons the test assembly carries.
The two contract animations are blank stand-ins, since the hinge cases never play them.

## `TCardOrdinal`

The number box on a card's position badge is covered as the user sees it.
The box repaints from the card's row painter, reached through the real item watcher of a list.
The commit gate that reads the typed text is covered in `TCardList` of the Conduct suite.
So these cases stand in for the engine's answer with a position change or none.
Opening the badge repaints the box with the stored number, whatever text it held before.
Typed text stays in the box while the badge is open.
Hiding the badge, as a cancel does, brings the stored number back.
A refused commit, past the end or unreadable, leaves the position unchanged and shows the stored number.
An accepted commit moves the card and shows the new number after the badge hides.
A position change while the badge is open repaints the box over the typed text.

## `TCardRow`

The shared row matcher keeps a card list's held rows across a repaint.
The rows are plain notifying objects, so the case needs no STA thread.
Two held rows and three drafts may carry the same id.
Each held row is matched once, in order, and takes its own draft's text.
The third draft finds no free held row, so a new row is made for it.
A match that ignored rows already taken handed every draft to the first row and dropped the second.

## `TCaretKey`

The chip caret keys are driven by name with no window or text box.
Each case proves one branch of `QCaret.QCaretEdgeApply` or `QCaret.QCaretStepApply` and the callbacks it reaches.

## `TEditorCardWalk`

The writing card lists are covered as Conduct's walk feeds them.
The walk is the editor's ordered steps, and a step carries its card, its fold and its caret facets.
The driver lists exactly the card steps of the walk, one row per card and in the walk's order.
So the driver decides no order or membership of its own.
A caret trail move with no draft change still writes the new anchor into the step's caret entry.
So the walk holds the caret the user left, with no draft change to carry it.
An opened entry's steps light the Undo button.
A walk change repaints the card lists on the GUI, with or without a draft change.
A settle the engine refuses on a Situation or Tag entry leaves the text typed.
The console does the same.

## `TLecternHinge`

The reading card's fold hinge is covered as the reading painter and the reading driver see it.
It builds its rows and runs its threads through the shared builders of `TCardHinge`.
The leaves carry no "see" lines, and only the heading case nests one collocation row.
A folded reading card hides its body and checks its hinge.
Its header rounds all four corners and drops its bottom line.
A folded card without a title shows its meaning as the peek, and the title and kind caption give way.
A hinge click bubbles to the card list and reaches the display's fold gate.
The gate is told whether the card is a collocation, so the fold is kept under the right kind.
It is refused while the display has no entry chosen, and the hinge returns to unchecked.
A click on a "see" line opens the collocation row it names.
It leaves no fold of that kind stored, and the display announces the card's part and place as sought.
The test listens to the compass's sought event, since the driver raises no notice of its own.
The test folds a collocation first and reads the stored folds back after the click.
A closed collocation row hides its body and checks its hinge.
It shows its phrase, its meaning on one line, its flags and its example and "also" markers.
An open row unchecks its hinge and wraps its meaning.
Its flags and markers give way, since the body shows the fields they summarise.
A meaning card with one homed collocation shows Conduct's ready caption and the homed list.

## `TLecternWord`

The word cursor of the reading card is driven by key presses on a Mention line in a bare surface.
Each case seeds two entries headed water and one sentence reading the water, then opens it in a real display.
The line is raised through preview and key events, so the cursor sees what a user would press.
The returned text is what the line paints with a background, which is the cursor character.
Right does nothing until Enter has started the cursor, and Enter then draws it on the first character.
The line text stays the sentence, and no lookup notice is raised by starting.
Enter after moving looks up the word under the cursor.
It raises one notice with the offer and the word start.
Both water entries are offered, and the route's lead is the text from the word's start to the cursor.
Escape clears the cursor, and a Right after it paints nothing.
The route keeps the unit it had, and Enter starts again from the first character.
The same class covers the card chips.
A chip of a record never saved is disabled, and a stored chip stays enabled.

## Stamp

Commit: `9377e657b395`
- `TBerthSeat.cs` `a99f5301812862be`
- `TCardHinge.cs` `523920c947648f34`
- `TCardOrdinal.cs` `ecf2476428289e78`
- `TCardRow.cs` `618531bdc90512ef`
- `TCaretKey.cs` `6eaf4b7930a95206`
- `TEditorCardWalk.cs` `e8cbd7f5a3860147`
- `TLecternHinge.cs` `67496750ce7f996b`
- `TLecternWord.cs` `68bd0cc4289e13e6`
