# PChronicleHost.cs

## `internal interface PChronicleHost`

What the window's undo and redo keys ask of whichever draft editor holds the keyboard.
Four editors answer: the entry form, the source form, the situation form and the example form.
Each flushes its own debounced request first, then asks the engine to step its chronicle.
The engine's draft bulletin brings the restored fields back through the same path a reload uses.
So nothing here draws anything by hand.
`PChronicleUpdate` settles the undo and redo buttons against what the engine can still step.
Each host runs it after every change update and every step.
So the buttons never lag the chronicle.

## `internal static class PChronicle`

The one wrap the four hosts put around their engine step.

## `internal static void PChronicleRun(Action step)`

Runs the step and keeps the caret at the end of the focused text box.
Restoring a field's text through a bulletin drops the caret to zero.
Bulletins fire synchronously on the calling thread, so the restore has happened when the step returns.
The caret is moved only when the same box still holds the keyboard.
