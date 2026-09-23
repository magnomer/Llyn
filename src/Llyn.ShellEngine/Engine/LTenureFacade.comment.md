# LTenureFacade.cs

## `internal sealed class LTenureFacade`

The engine's facade for Tenure, which starts a draft hold for a vista or named subject.

## `public LTenureFacade(LEngine engine)`

Stores the engine whose draft operations the facade uses.

## `internal int LEngineTenureDelay`

The quiet a tenure waits for before writing what was deferred, in milliseconds.
Read and written volatile because a tenure's timer reads it off the UI thread.

## `internal LTenure LEngineTenureStart(LVista vista, long? id)`

Starts a tenure over the vista's tab and subject.
A vista with no subject cannot name what to hold, so it is refused.

## `internal LTenure LEngineTenureStart(string origin, LSubject subject, long? id)`

Starts a draft of the kind the subject names, on a stored record or on nothing, and returns its tenure.
Only an entry, example, situation, reference or author can be held, so another subject is a caller's error.
A missing stored record refuses as the underlying start does.
