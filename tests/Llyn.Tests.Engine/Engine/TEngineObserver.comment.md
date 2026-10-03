# TEngineObserver.cs
Hash: `612db4ae2c0a859f`

## `public sealed class TEngineObserver`

Covers the engine's subscription as a delegate rather than a contract.
A lambda attached counts one bulletin when a setting changes.
A method group detached with the delegate it was attached with falls silent, since equality is target plus method.
The same method group attached twice is one subscriber, so a re-attached surface keeps one voice.
A tenure keeps its state until an announcement moves the engine revision, so its readings are covered here too.

## `public void ObserverAttach_SettingsChanged_CountsOne()`

A lambda attached to the engine hears one bulletin when a setting is saved.
It is the plain case of a surface subscribing, and every other observer fact builds on it.

## `public void ObserverDetach_SameDelegate_CountStays()`

Detaching the delegate that was attached silences it, so the bulletins stop at the one already heard.
A surface that closes must stop being called, or it would be told about state it no longer shows.

## `public void ObserverAttach_SameDelegateTwice_CountsOnce()`

Attaching the same method twice leaves one subscriber, so one change reaches it once.
A surface that attaches again on reopening would otherwise repeat every announcement.

## `public void TenureStateRead_NothingChanged_AnswersTheKeptStateAgain()`

A second reading with nothing written between answers the very same state, so a refresh skips the disk.

## `public void TenureStateRead_RequestApplied_ComputesAFreshStateThatSeesTheChange()`

An applied request announces a draft change, so the next reading computes and sees the edit with an undo waiting.

## `public void TenureStateRead_Undone_ComputesAFreshStateThatSeesTheStepBack()`

An undo announces the restored draft, so the next reading sees it unchanged with a redo waiting.

## `public void TenureStateRead_SameRecordStoredByAnotherTenure_ComputesAFreshStateThatSeesTheStoredRecord()`

A store by one tenure moves the stored record under another tenure on it.
The watching tenure must then compute again, since its draft now differs from what is stored.

## `public void TenureStateRead_Cancelled_AnswersTheEndedStateInsteadOfTheKeptOne()`

A cancelled tenure answers the fixed ended state, never the state it kept while alive.
