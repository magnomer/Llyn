# TEngineObserver.cs
Hash: `187689938e19be02`

## `public sealed class TEngineObserver`

Covers the engine's subscription as a delegate rather than a contract.
A lambda attached counts one bulletin when a setting changes.
A method group detached with the delegate it was attached with falls silent, since equality is target plus method.
The same method group attached twice is one subscriber, so a re-attached surface keeps one voice.
A tenure keeps its state until an announcement moves the engine revision, so its readings are covered here too.

## `public void TenureStateRead_NothingChanged_AnswersTheKeptStateAgain()`

A second reading with nothing written between answers the very same state, so a refresh skips the disk.

## `public void TenureStateRead_RequestApplied_ComputesAFreshStateThatSeesTheChange()`

An applied request announces a draft change, so the next reading computes and sees the edit and its undo.

## `public void TenureStateRead_Undone_ComputesAFreshStateThatSeesTheStepBack()`

An undo announces the restored draft, so the next reading sees it unchanged with a redo waiting.

## `public void TenureStateRead_SameRecordStoredByAnotherTenure_ComputesAFreshStateThatSeesTheStoredRecord()`

A store by one tenure moves the stored record under another tenure on it.
The watching tenure must then compute again, since its draft now differs from what is stored.

## `public void TenureStateRead_Cancelled_AnswersTheEndedStateInsteadOfTheKeptOne()`

A cancelled tenure answers the fixed ended state, never the state it kept while alive.
