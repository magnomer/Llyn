# PCaret.cs

## `internal sealed class PCaret : Adorner`

The text cursor every input field is typed against.
WPF draws its own, but only its color can be set — its width comes from a Windows-wide setting and its blink is a hard on-off.
So the framework caret is made transparent and this one is drawn over the field instead.
It is two pixels wide with rounded ends, painted in the theme accent, and it fades rather than snaps.

### `field.CaretBrush = Brushes.Transparent;`

The framework caret is hidden, not removed.
The real caret still holds the position the IME composes at.
Japanese, Mandarin, and Cantonese input would otherwise place their candidate window wrongly.

### `internal static void PCaretHook()`

One class handler serves every text box in the program, including the ones inside control templates.
A field is adorned the first time it takes keyboard focus, so a field never opened costs nothing.

### `AdornerLayer? layer = AdornerLayer.GetAdornerLayer(field);`

A field outside any adorner layer is left with no caret of ours.
It keeps the framework caret, because nothing was made transparent yet.

### `private void PCaretLayoutHandle(object? sender, EventArgs e)`

Scrolling, resizing, and font changes all move the caret without changing the text.
Layout is the one signal that catches every such move.
It fires often, so the position is compared first and nothing is redrawn when it stands still.

### `private void PCaretOffsetApply(double offset)`

A caret that jumps is hard to follow, so a move along one line is slid instead.
The offset starts at where the caret was and eases to zero, which is where it now belongs.
A move to another line, or a long jump, is not slid — the eye loses a caret that travels too far.

### `if (typed || moved || !_pCaretBlink)`

Typing holds the caret solid, because a caret that blinks mid-word is read as a stutter.
Each keystroke restarts the cycle, so the blink resumes only once typing stops.

### `Brush face = _pCaretField.TryFindResource("Theme.Accent") as Brush ?? Brushes.Black;`

The accent is read at each draw, so a theme change is picked up without rebuilding the caret.
