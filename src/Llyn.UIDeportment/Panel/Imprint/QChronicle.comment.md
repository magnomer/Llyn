# QChronicle.cs
Hash: `66432adee340042e`

## `internal static class QChronicle`

The one wrap every host puts around its engine step.
It also keeps the hosts that drive a veneer surface, since such a surface is not a host itself.
It owns the three key chords that walk a draft's chronicle: Ctrl+Z back, Ctrl+Y or Ctrl+Shift+Z forward.
The window subscribes the chords once, so every draft editor answers them the same way.
Which editor answers is decided by where the keyboard is, not by which panel is showing.

## `private static readonly ConditionalWeakTable<DependencyObject, QChronicleHost> QChronicleHold = [];`

Each surface's driver, held weakly so a dropped surface frees its entry.

## `internal static void QChronicleIntroduce(DependencyObject surface, QChronicleHost host)`

Names the driver that answers the undo and redo keys for everything inside the surface.

## `internal static QChronicleHost? QChronicleRead(DependencyObject element)`

The driver attached to this element, or null when none was.

## `internal static void QChronicleCaretRefine(Action step)`

Runs the step and keeps the caret at the end of the focused text box.
Restoring a field's text through a bulletin drops the caret to zero.
Bulletins fire synchronously on the calling thread, so the restore has happened when the step returns.
The caret is moved only when the same box still holds the keyboard.

## `private static QChronicleHost? QChronicleFocusRead()`

Climbs from the focused element to the first ancestor that answers undo and redo.
A veneer surface answers through the driver attached to it, as the situation form does.
The situation and example forms each hold an entry form inside them.
Climbing meets the nearer of the two first, which is the one the user is typing in.
A focused element outside the visual tree climbs the logical tree instead, so a text run still finds its form.
Nothing focused, or nothing above it that answers, says the chord is not ours.

## `internal static void QChronicleKeyObserve(object sender, KeyEventArgs e)`

Turns the chord into an undo or a redo on the host found, and swallows the key when one was.
The window subscribes it on preview, so it beats the text box's own undo.
The theme has switched that undo off anyway.
A chord with no host under it falls through untouched.
