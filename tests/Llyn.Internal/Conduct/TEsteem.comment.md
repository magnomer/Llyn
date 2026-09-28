# TEsteem.cs

## `public sealed class TEsteem`

Covers the marks an editor sets on its stored entry.
A grasp write reads back from the engine and words its label.
A fresh draft ignores the write, words nothing and still announces a re-read.
A favourite mark and its clearing read back the same way, each announced.
The grasp limit comes from the engine, and a fresh draft reads no frequency chip.

## `private static LEntry TEsteemEntryPrepare(LEngine engine)`

Stores one English entry the editor can open.

## `private static CEditor TEsteemEditorPrepare(LEngine engine, string tab)`

Builds an editor over real outlets, bound to a fresh vista of `tab`.
