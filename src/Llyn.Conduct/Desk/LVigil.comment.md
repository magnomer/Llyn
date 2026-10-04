# LVigil.cs
Hash: `1a3cb2985ea5a680`

## `internal sealed class LVigil`

The observers one desk keeps by subject and puts on every tenure it starts.
A driver registers once and never sees the tenure.
The vigil re-attaches its observers on every start.

## `internal LVigil(CDesk desk)`

Only the desk builds one, over itself.

## `internal void LVigilObserverAttach(CSubject subject, Action<CBulletin> observer)`

Registers an observer for the tenure-wide subjects.
It maps, copies and attaches as `LVigilDraftAttach` describes.

## `internal void LVigilDraftAttach(CSubject subject, Action<CBulletin> observer)`

Registers an observer for the draft-level subjects.
An observer is a delegate over a Conduct bulletin, which the vigil copies from each engine bulletin.
The subject maps through `CCatalog.LCatalogSubjectRead`, so a subject it does not list throws.
A tenure already held gets the observer at once.

## `internal void LVigilEntryAttach(CSubject subject, Action<CBulletin> observer)`

Registers an observer for the entry-level subjects.
It maps, copies and attaches as `LVigilDraftAttach` describes.

## `internal void LVigilApply(LTenure started)`

Puts every registered observer on the tenure the desk just started.
The observers are on the tenure before the desk announces the start.
