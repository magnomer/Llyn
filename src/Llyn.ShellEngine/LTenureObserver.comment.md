# LTenureObserver.cs

A tenure subscribes to the engine for its lifetime and owns the shell's subject-specific observers.
General observers receive every notice of their subject, while draft observers require the held draft identity.
Entry observers require a stored entry identity, because frequency and related notices do not name the draft.
Preparation drops the held draft's own notices, because the caller reads the newest draft from the return value.
A replayed notice would show the same draft twice, and the editor's show is its costliest step.
The scope unwinds on failure and returns the newest draft after successful preparation.
Cancel and successful finish detach the engine subscription and release the observer list.
