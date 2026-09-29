# PProffer.cs

## `public partial class PEditor`

The dropdown of stored rows a typed wording may already name, and the rows it offers.
It serves the Situation caret, the Register caret and the citation field of a sentence row alike.
It is one popup the editor owns rather than one per card, for the reason the translation dropdown is.
Only one field is being typed into at a time.
The popup is retargeted at the caret that opened it.

A Situation is a workspace entity, not a word on a card.
It carries a description, a kind and a usage count.
Two cards naming the same wording should name the same Situation.
Without this the wording would be written twice and the workspace would hold two Situations that read alike.

The list opens as the wording is typed rather than only when it is committed.
A user cannot pick from a catalogue they were never shown.

The dropdown keeps no note of the field it serves.
A pick reads the field that holds focus.
The dropdown never takes focus, and it shuts when that field is left.

## `private readonly PProfferTemplate _pProfferTemplate`

The offered row dictionary, held so the editor merges it into its resources.

## `private void PProfferApply(FrameworkElement container, object item, string? _)`

Fills one offered row from its ready pieces and count, and subscribes its press.
The miss Refine is subscribed before the pick Observe, so each hears the press in its role.

## `private void PProfferMissRefine(object sender, MouseButtonEventArgs e)`

A press that reaches no offered row shuts the dropdown, with no gate involved.

## `private void PProfferPickObserve(object sender, MouseButtonEventArgs e)`

Hands the row the pointer chose to the one gate the focused field serves.
A Register caret links the stored Register and a Situation caret links the stored Situation, each at the caret.
The citation field cites the chosen Source on its sentence row through `CSentenceCitationSet`.
The press is heard before focus moves, so that field is still the one the list was opened for.
The list then shuts, and a caret empties.
The clerk skips a row the card already carries.

## `private void PProfferRegisterRefine(PCard card, CProffer offer)`

Paints the register gate's answer: the kept text into the card's entry, then the dropdown.
The dropdown opens when the answer says so and shuts otherwise.

## `private void PProfferSituationRefine(PCard card, CProffer offer)`

Paints the situation gate's answer: the kept text into the card's entry, then the dropdown.
The dropdown opens when the answer says so and shuts otherwise.
The gate matches the wording anywhere inside a stored title, so "the situation" finds "This is the situation".
Rows come most used first, so the settled wordings stand first, and the list stays shut when nothing matches.

## `private void PProfferCitationRefine(TextBox box, CProffer offer)`

Paints the citation gate's answer as the dropdown against the citation field.
The dropdown opens when the answer says so and shuts otherwise.
The field keeps its own text, since typing a citation writes nothing.
The gate offers nothing for the byline the row already cites.

## `private void PProfferOpenRefine(TextBox? box, IReadOnlyList<CProfferRow> rows)`

Fills the dropdown with the ready rows as they come, places it against the caret and opens it.
Every find's Refine shares it, so the popup looks the same whichever field opened it.

## `private void PProfferShutRefine()`

Shuts the dropdown and forgets its rows and its selection.

## `private void PProfferAttach()`

Hands the dropdown list its rows and fill, gives the dropdown its placement, and clears its selection as it shuts.

## Inline notes

### `private void PProfferKeyRefine(object sender, KeyEventArgs e)`

The dropdown never takes focus, so the field's keys drive it while it stands open.
Escape shuts it, and the arrows walk its rows, with no gate involved.
Down from nothing selects the first row and up selects the last.
Each field subscribes it before its own key handlers, so a key it takes ends there.

### `private void PProfferCloseRefine(object? sender, EventArgs e)`

A shut dropdown keeps no selected row, however it was shut.
So enter reaches a stored row only while the list stands open.

### `private void PProfferKeyObserve(object sender, KeyEventArgs e)`

Enter on a selected row hands that stored row to the gate the field serves, as a press does.
Nothing is selected while the list merely stands open, so enter still commits the typed wording.
The user reaches the list with the arrows, and only then does enter take a row.

### `PProffer.PlacementTarget = PProfferFrameFind(box) ?? box ?? (UIElement)PContents;`

The frame inside the caret, because that is the edge the dropdown is read against.
The caret it is drawn for falls back in, then the pane.
The list still opens when the frame is not built yet.
The sheet's minimum width is bound to that target's width, so it follows a resize while open.

### `private static CustomPopupPlacement[] PProfferPlace(Size popup, Size target, Point offset)`

The dropdown is placed against the frame by hand, from the frame's own corner.
Placing it under the frame's edge instead left it short of that edge and over the caret.
The gutter the surface keeps for its shadow is taken back on both counts.
The list reads as the frame's own edge continued.
The second placement puts it above the frame, for a caret near the foot of the screen.
