# TEsteem.cs
Hash: `792aeace50d0284f`

## `public sealed class TEsteem`

Covers the marks an editor sets on its stored entry.
A grasp write reads back from the engine and words its label.
The standing step pressed again clears the grasp.
A fresh draft ignores the write, words nothing and still announces a re-read.
A favourite mark and its clearing read back the same way, each announced.
The grasp limit equals the Core grasp step on a draft and a stored editor.
A fresh draft reads no frequency chip.
Any engine grasp limit reads unchanged, except a negative one, which reads zero.
Any stored step the engine answers reads between zero and that limit.

## `private static long TEsteemStoredPrepare(LEngine engine)`

Stores one English entry, and answers its id, so the marks open it over a fake grasp port.

## `private static LEntry TEsteemEntryPrepare(LEngine engine)`

Stores one English entry the editor can open.

## `private static CEditor TEsteemEditorPrepare(LEngine engine, string tab)`

Builds an editor over real outlets, bound to a fresh vista of `tab`.
