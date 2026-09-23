# LCaret.cs

## `public static class LCaret`

The caret rules every chip entry shares, kept where a test can reach them without a window.

## `public static bool LCaretKeyApply(string key, int caret, int length, int selection, Action<int> remove, Func<int, bool> move, Action place)`

The keys every chip caret answers the same way, decided in one place.
The key arrives as its WPF name, such as `Back` or `Left`.
A selection leaves every key to the text box, so ordinary editing is untouched.
Backspace at the start of the entry drops the chip before it.
Delete at the end of the entry drops the chip after it.
The arrow keys walk an empty entry past a chip, and the caller puts the caret back.
So a chip is crossed the way a character is, in either direction.
The answer says whether the key was taken.
