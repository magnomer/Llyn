# QVigil.cs

## `public sealed class QVigil`

The observers one desk keeps by subject and puts on every tenure it starts.
The veneer registers once and never sees the tenure, which is what re-attaches per start.

## `internal QVigil(LDesk desk)`

Only the desk builds one, over itself.

## `public void QVigilDraftAttach(CSubject subject, Action<CBulletin> observer)`

Registers an observer for the draft-level subjects.
An observer is a delegate over a Conduct bulletin, which the vigil copies from each engine bulletin.
A tenure already held gets the observer at once.
`QVigilObserverAttach` and `QVigilEntryAttach` do the same for the tenure-wide and entry-level subjects.

## `internal void QVigilApply(LTenure started)`

Puts every registered observer on the tenure the desk just started.
The observers are on the tenure before the desk announces the start.
