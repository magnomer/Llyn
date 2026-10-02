# TSession.cs
Hash: `747bf32585e58ba2`

## `public sealed class TSession`

Covers one side panel's draft session and the quit decision across editors.
A save stores a changed draft and shows the stored id, and keeps an unchanged one.
A refused ready check keeps the draft, and a close stores without asking it.
While the editor shows, finish and close go to the editor, and an unchanged save stores nothing.
The quit decision asks only over unsaved work, and every editor hears the one answer.
