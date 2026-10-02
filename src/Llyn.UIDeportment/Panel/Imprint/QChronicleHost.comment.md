# QChronicleHost.cs
Hash: `7351a1a9a0bf31e1`

## `internal interface QChronicleHost`

What the window's undo and redo keys ask of whichever draft editor holds the keyboard.
Four editors answer: the entry form, the source form, the situation form and the example form.
Each flushes its own debounced request first, then asks the engine to step its chronicle.
The engine's draft bulletin brings the restored fields back through the same path a reload uses.
So nothing here draws anything by hand.
The desk's own state notice settles the undo and redo buttons after every step.

## `internal static class QChronicle`

The one wrap the hosts and the guild panel put around their engine step.
It also keeps the hosts that drive a veneer surface, since such a surface is not a host itself.

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
