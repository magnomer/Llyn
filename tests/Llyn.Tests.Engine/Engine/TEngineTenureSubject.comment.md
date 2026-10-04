# TEngineTenureSubject.cs
Hash: `f6a05df885035ad7`

## `public sealed class TEngineTenureSubject`

Covers how a finished tenure commits each subject it can hold, on a fresh workspace.
A changed example, entry, situation or reference is stored, its draft dropped and its id answered.
An author tenure is ready only once its name is written.
A tenure on a stored Author renames it in place, and a blank rename is refused.
Each test starts its tenure on an engine from `TWorkspace.TWorkspacePrepare`.
Only the ready check sets the delay to `TEngineTenure.TTenureHold`, and the rest set it to zero.

## `public void TenureFinish_Changed_CommitsExample()`

Finishing a changed example tenure stores the example, drops the draft and answers the stored id.

## `public void TenureFinish_Changed_CommitsEntry()`

Finishing a changed entry tenure stores the entry, drops the draft and answers the stored id.

## `public void TenureFinish_Changed_CommitsSituation()`

Finishing a changed situation tenure stores the situation the same way.
The title names the Situation the draft holds, as the scenario's own edit does.

## `public void TenureFinish_Changed_CommitsReference()`

Finishing a changed reference tenure stores the reference the same way.

## `public void TenureFinish_Changed_CommitsAuthor()`

Finishing a changed author tenure creates the Author under the deferred name.

## `public void TenureReadyCheck_AuthorNamedLater_ReadyOnceTheNameIsWritten()`

A fresh author tenure is not ready with its blank name, and a deferred name makes it ready.
The check writes the queued name first, so the long hold never has to pass.

## `public void TenureFinish_Renamed_UpdatesAuthor()`

An author tenure on a stored Author reads its name, stays unchanged until a name is deferred, then renames it.

## `public void TenureFinish_Unnamed_RefusesAuthor()`

Finishing a tenure that blanks a stored Author's name throws the name-missing `LRefusal`.
The stored name stays as it was.
