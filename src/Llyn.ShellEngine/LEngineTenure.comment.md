# LEngineTenure.cs

## `public sealed partial class LEngine`

Where a panel asks for a tenure, the engine's hold on one draft from start to commit or cancel.
The draft primitives in `LEngineDraftHold.cs` stay as they are and the tenure drives them.
Before this the four editor panels each copied the same start, defer, save, check and commit sequence.
Now the engine runs that sequence once, and a panel keeps only the tenure and its controls.

## `private int _lEngineTenureDelay = 250;`

The quiet a tenure waits for before it writes what was deferred, in milliseconds.
Long enough that ordinary typing writes once rather than once per letter.
Short enough that a crash costs a word, not a sentence.

## `internal int LEngineTenureDelay`

The seam a test sets the delay through, zero for writes that land at once.
Read and written volatile, because a tenure's timer reads it off the UI thread.

## `public LTenure LEngineTenureStart(string origin, LSubject subject, long? id)`

Starts a draft of the kind the subject names, on a stored record or on nothing, and returns its tenure.
Only an entry, example, situation, source or author can be held, so another subject is a caller's error.
A missing stored record refuses as the underlying start does.
