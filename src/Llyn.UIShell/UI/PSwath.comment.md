# PSwath.cs

## `internal static class PSwath`

The band of text a reader drags across in the reading view, highlighted so it can be copied.
A text block cannot be selected on its own, and the framework offers no switch for it.
The editor a text box uses is internal, so it is reached by reflection and set on each block read-only.
Runs, links and styles stay as drawn, because the block itself is not replaced.
Every block under the display is covered, so headword, readings, cards and note all select.

### `private static readonly bool PSwathReady`

A framework build that renamed any of the internal members leaves every block as it was.
Nothing is copied then, but nothing breaks either.

### `internal static void PSwathHook()`

The copy command and the mouse handlers are registered once for every text block in the program.
A block without an editor answers none of them, so a block outside the display costs nothing.
Blocks are attached as they load, since the cards draw theirs from templates long after the entry is shown.

### `private static bool PSwathDisplayCheck(DependencyObject block)`

Only a block inside the reading view is made selectable.
A block inside a button stays a button's face, because a drag would swallow the click it exists for.
Tooltips and popups hang from their own root and never reach the display, so they are left alone.

### `private static void PSwathAttach(TextBlock block)`

The editor is created against the block's own text container and told to read only.
The block must be focusable, or the editor never sees a key, and a focus rectangle would frame it.
The editor is kept on the block so a block that loads twice is not attached twice.
