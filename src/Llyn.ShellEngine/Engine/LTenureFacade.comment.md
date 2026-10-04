# LTenureFacade.cs
Hash: `c7d7ee4457051f6e`

## `internal sealed class LTenureFacade`

The engine's facade for Tenure, which starts a draft hold for a vista or named subject and commits it.

## `public LTenureFacade(LEngine engine)`

Stores the engine whose draft operations the facade uses.

## `internal int LEngineTenureDelay`

The quiet a tenure waits for before writing what was deferred, in milliseconds.
Read and written volatile, so a change from one thread reaches the deferrals made on another.

## `internal LTenure LEngineTenureStart(LVista vista, long? id)`

Starts a tenure over the vista's tab and subject.
A vista with no subject cannot name what to hold, so it is refused.

## `internal LTenure LEngineOccurrenceStart(LVista vista, long? situation)`

Starts a fresh entry over the vista and links the Situation to its first card before anyone sees it.
The link is one `LRequestSituationPick` applied at once, so the clerk's pick rule applies unchanged.
A null Situation leaves the fresh entry blank.

## `internal LTenure LEngineQuotationStart(LVista vista, long? example)`

Starts a fresh entry over the vista and cites the Example in its first sentence before anyone sees it.
The citation is one `LRequestSentenceExample` applied at once, so the clerk's rule applies unchanged.
A null Example leaves the fresh entry blank.

## `internal LTenure LEngineFootnoteStart(LVista vista, long? reference)`

Starts a fresh entry over the vista and cites the Source in its first sentence before anyone sees it.
The citation is one `LRequestSentenceReference` applied at once, so the clerk's rule applies unchanged.
A null Source leaves the fresh entry blank.

## `internal LTenure LEngineMembershipStart(LVista vista, long? tag)`

Starts a fresh entry over the vista and puts the Tag on its first card before anyone sees it.
The tag is one `LRequestTagPick` applied at once, so the clerk's pick rule applies unchanged.
A null Tag leaves the fresh entry blank.

## `internal LTenure LEngineCohortStart(LVista vista, long? register)`

Starts a fresh entry over the vista and puts the Register on its first card before anyone sees it.
The register is one `LRequestRegisterPick` applied at once, so the clerk's pick rule applies unchanged.
A null Register leaves the fresh entry blank.

## `internal LTenure LEngineTenureStart(string origin, LSubject subject, long? id)`

Starts a draft of the kind the subject names, on a stored record or on nothing, and returns its tenure.
Only an entry, example, situation, reference or author can be held, so another subject is a caller's error.
A missing stored record refuses as the underlying start does.

## `internal long LEngineTenureCommit(LSubject subject, long id)`

Commits the draft through the part the subject names and answers the stored record's id.
It mirrors the start beside it, so one owner maps each subject to its engine part.
A tenure is only ever started for these subjects, so another one is a broken invariant, not a caller's error.
