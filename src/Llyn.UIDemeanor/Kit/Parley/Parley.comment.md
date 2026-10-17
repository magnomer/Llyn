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

## `BSParleyWidth`

The box is 58 cells wide, or the width of its body when it has one.
The box shrinks to the console width less 2 cells.

## `BSParleyKeyObserve`

A focused body sees the key first, and a key it takes goes no further.
Enter presses the lit button, and a button's own key presses that button.
A disabled button never answers, whether by Enter or by its key.
Left and Right move the lit button.
Without a body, Tab does the same, backwards with Shift.
The lit button skips disabled ones.
With a body, Tab switches focus between the body and the buttons.

## `BSParleyDraw`

Places the blueprint twice, first to measure the text and then at its final height and position.
The box is centred, or put at the anchor and clamped inside the console.
The height follows the text rows and the body, and never exceeds the console less 2 rows.
A line cuts at its width.
A path line breaks after `\` and `/`, so a long path wraps at its separators.
The box draws a shadow of one column to the right and one row below it.
The title is centred on the top border and omitted when it does not fit.
Buttons are `[ label ]`, spaced by 5 cells, or by 1 when they do not fit.
A disabled button draws in the role `disabled`, and the lit one in `lit`.
A body draws below the text and may print its position on the bottom border.

## `BSParleyBody`

What a box can hold besides text, such as a field.
It tells its width, height and position text, observes keys and draws itself.
Observing returns true when the body took the key.

## `BSParleyButton`

A label, an optional key and an enabled flag.
The enabled flag is the one writable member, since a body switches it as its input changes.

## `BSParleyLine`

One text line and whether it is a path.
A path line may break after a separator.

## Stamp

Commit: `9a85a3ab82db`
- `BSParley.cs` `75f5cc529c1ff9e4`
- `BSParleyBody.cs` `a4a8e3b5f6118120`
- `BSParleyButton.cs` `d4e092b24eff93d0`
- `BSParleyLine.cs` `34f3fd75cc4134ab`
