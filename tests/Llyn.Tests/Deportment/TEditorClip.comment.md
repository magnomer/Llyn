# TEditorClip.cs

## `public sealed class TEditorClip`

Covers the editor's recording search over its desk with no dispatcher in the way.
A lambda handed to the clip start receives the source step, the recording and the end of a one-source pack.
So a deportment hands a delegate through the desk and the tenure, and the engine streams to it as data.
The steps are collected under a lock, since the source answers on a worker thread.
The clip deportment turns each step into one notice, source, recording and end in turn.
