# TSession.cs
Hash: `be4f9329241533d8`

## `public sealed class TSession`

Covers one side panel's draft session and the quit decision across editors.
A save stores a changed draft and shows the stored id, and keeps an unchanged one.
A refused ready check keeps the draft, and a close stores without asking it.
While the editor shows, finish and close go to the editor, and an unchanged save stores nothing.
A cancel drops the held draft and announces it, and undo and redo step the desk.
A failed undo of the own desk shows the scope's `HoldFailed` key once, not twice.
With the editor hidden, finish ends the own desk.
The quit decision asks only over unsaved work, and every editor hears the one answer.
