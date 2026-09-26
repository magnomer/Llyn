# TWorkspaceClerk.cs

## `public sealed class TWorkspaceClerk`

The static open sequence of the workspace clerk over the fake rig, without SQLite.

## `public void WorkspaceRescueCreate_DoctorRefuses_ThrowsBeforeAnythingIsRead()`

A rig whose doctor cannot open the database throws out of the first open step.
The engine therefore keeps its old clerks, which is the unopenable-target contract.

## `public void WorkspaceSettingsRead_NothingStored_AnswersTheFallbackUnsettled()`

A rig with no stored settings answers the fallback and says so, so the caller writes it down.

## `public void WorkspaceSettingsRead_SettingsStored_AnswersTheStoredOnesSettled()`

Stored settings win over the fallback.

## `public void WorkspaceNoticeRead_RefusalWrappedTwice_AnswersTheReason()`

The refusal reason is found through two wrapping exceptions, and a bare exception answers nothing.

## `private sealed class TDoctorFake : LDoctorVault`

A doctor that refuses to open, standing in for a database folder that is a file.
