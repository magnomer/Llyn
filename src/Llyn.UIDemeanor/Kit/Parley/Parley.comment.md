# Parley

The console's sublevel box: a framed question, notice or form with buttons, drawn over the window.
It is the console's answer to a modal dialog, and it knows no program.

## `BSParley`

One box with a title, a role, text lines, buttons and an optional body.
It takes a blueprint, which must hold `ASParleyText` and `ASParleyButton`.
The safe index must lie inside the buttons, or the constructor throws.
The answer is the index of the pressed button, held in a task that completes once.
Escape and F10 answer the safe index unless a focused body takes them, so a user can leave.
The box holds its own state, a lit button and a focus.
It builds its own key table from the buttons' keys and the standard keys.
The window reads that table for the legend and help.
The body's own keys come first, but only while the body holds the focus.
With the focus on the buttons, help names the button keys and none of the body's.
It also carries a key focus, defaulting to the box focus, so help can take the help focus.

## `BSParleyWidth`

The box is 58 cells wide, or the width of its body when it has one.
The box shrinks to the console width less 2 cells.

## `BSParleyKeyObserve`

A focused body sees the key first, and a key it takes goes no further.
Enter presses the lit button, and a button's own key presses that button.
A key row matches its modifiers exactly, so Ctrl+Tab does nothing.
A disabled button never answers, whether by Enter or by its key.
Left and Right move the lit button.
Without a body, Tab does the same, backwards with Shift.
The lit button skips disabled ones.
With a body, Tab switches focus between the body and the buttons.
When that hands the focus to the body, the body is told the direction through `BSParleyBodyResume`, backwards on Shift.

## `BSParleyDraw`

Places the blueprint twice, first to measure the text and then at its final height and position.
The box is centred, or put at the anchor and clamped inside the console.
The height follows the text rows and the body, and never exceeds the console less 2 rows.
A line wraps at the text width, and rows past the text part are cut.
A path line breaks after `\` and `/`, so a long path wraps at its separators.
The box draws a shadow of one column to the right and one row below it.
The title is centred on the top border and omitted when it does not fit.
Buttons are `[ label ]`, spaced by 5 cells, or by 1 when they do not fit.
A disabled button draws in the role `disabled`, and the lit one in `lit`.
A body draws below the text and may print its position on the bottom border.
The box and its body count cells with one measure handed in by the window, never one of their own.

## `BSParleyBody`

What a box can hold besides text, such as a field or the help pages.
It tells its width, height and position text, observes keys and draws itself.
It may also offer a key table of its own, which the box lists in front of its keys.
It lists them only while the body holds the focus.
Drawing receives the width measure, so a body never builds its own.
It also receives the rows left in the text part under the text lines and their gap, never below zero.
The path prompt hands its path field one row.
Observing returns true when the body took the key.
Resuming tells the body the focus has come back to it and from which side.
The default does nothing, so a body with one part ignores it.
A body with several parts, such as the path prompt, uses it to pick the part that opens first.

## `BSParleyButton`

A label, an optional key with its legend, and an enabled flag.
The legend is a catalog key, required whenever there is a key.
The key's row dims while the button is disabled.
The enabled flag is the one writable member, since a body switches it as its input changes.

## `BSParleyLine`

One text line and whether it is a path.
A path line may break after a separator.

## Stamp

Commit: `cb8ab7e8feb4`
- `BSParley.cs` `f3ff3b2f505f7065`
- `BSParleyBody.cs` `c73380869b4abf3e`
- `BSParleyButton.cs` `5228b2831c1f9269`
- `BSParleyLine.cs` `34f3fd75cc4134ab`
