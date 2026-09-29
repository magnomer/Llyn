# QCaret.cs

## `internal sealed class QCaret : Adorner`

The text cursor every input field is typed against.
WPF draws its own, but only its color can be set.
Its width comes from a Windows-wide setting and its blink is a hard on-off.
So the framework caret is made transparent and this one is drawn over the field instead.
It is two pixels wide with rounded ends, painted in the theme accent, and it fades rather than snaps.

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

### `private void QCaretLayoutHandle(object? sender, EventArgs e)`

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

## `internal static bool QCaretKeyApply(string key, int caret, int length, int selection, Action<int> remove, Func<int, bool> move, Action place)`

The keys every chip caret answers the same way, decided in one place.
The key arrives as its WPF name, such as `Back` or `Left`.
A selection leaves every key to the text box, so ordinary editing is untouched.
Backspace at the start of the entry drops the chip before it.
Delete at the end of the entry drops the chip after it.
The arrow keys walk an empty entry past a chip, and the caller puts the caret back.
So a chip is crossed the way a character is, in either direction.
It runs the edge keys first and the arrow keys after.
Callers that hear every chip key in one handler use it.

## `internal static bool QCaretEdgeApply(string key, int caret, int length, int selection, Action<int> remove)`

The edge keys alone: Backspace at the start and Delete at the end hand the step to `remove`.
A caller that hears the erase keys on their own uses it, and a selection still leaves the key alone.

## `internal static bool QCaretStepApply(string key, int length, int selection, Func<int, bool> move, Action place)`

The arrow keys alone: an empty entry is walked past a chip, and `place` puts the caret back.
A caller that hears the caret moves on their own uses it.
The answer says whether the key was taken.
