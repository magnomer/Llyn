# QCaret.cs
Hash: `9944fb03dcc1c00e`

## `internal sealed class QCaret : Adorner`

The text cursor every input field is typed against.
WPF draws its own, but only its color can be set.
Its width comes from a Windows-wide setting and its blink is a hard on-off.
So the framework caret is made transparent and this one is drawn over the field instead.
It is at least two pixels wide with rounded ends, painted in the theme accent.
It fades rather than snaps.

It also holds the caret rules every chip entry shares, kept where a test can reach them without a window.

### `field.CaretBrush = Brushes.Transparent;`

The framework caret is hidden, not removed.
The real caret still holds the position the IME composes at.
Japanese, Mandarin, and Cantonese input would otherwise place their candidate window wrongly.

### `internal static void QCaretHook()`

One class handler serves every text box in the program, including the ones inside control templates.
A field is adorned the first time it takes keyboard focus, so a field never opened costs nothing.

### `AdornerLayer? layer = AdornerLayer.GetAdornerLayer(field);`

A field outside any adorner layer is left with no caret of ours.
It keeps the framework caret, because nothing was made transparent yet.

### `private void QCaretLayoutRefine(object? sender, EventArgs e)`

Scrolling, resizing, and font changes all move the caret without changing the text.
Layout is the one signal that catches every such move.
It fires often, so the position is compared first and nothing is redrawn when it stands still.

### `private void QCaretOffsetApply(double offset)`

A caret that jumps is hard to follow, so a move along one line is slid instead.
The offset starts at where the caret was and eases to zero, which is where it now belongs.
A move to another line, or a long jump, is not slid.
The eye loses a caret that travels too far.

### `if (typed || moved || !_qCaretBlink)`

Typing holds the caret solid, because a caret that blinks mid-word is read as a stutter.
Each keystroke restarts the cycle, so the blink resumes only once typing stops.

### `Brush face = _qCaretField.TryFindResource("Theme.Accent") as Brush ?? Brushes.Black;`

The accent is read at each draw, so a theme change is picked up without rebuilding the caret.

## `internal static bool QCaretEdgeApply(string key, int caret, int length, int selection, Action<int> remove)`

The erase keys every chip caret answers the same way, decided in one place.
The key arrives as its WPF name, such as `Back` or `Delete`.
A selection leaves every key to the text box, so ordinary editing is untouched.
Backspace at the start of the entry hands the step before it to `remove`.
Delete at the end of the entry hands the step after it to `remove`.
Each chip entry's erase Observe calls it and ends in its remove gate.

## `internal static bool QCaretStepApply(string key, int length, int selection, Func<int, bool> move, Action place)`

It takes the arrow keys alone.
An empty entry is walked past a chip, and `place` puts the caret back.
So a chip is crossed the way a character is, in either direction.
Each chip entry's caret Refine calls it after the erase Observe, which ignores the arrows.
The answer says whether the key was taken.
