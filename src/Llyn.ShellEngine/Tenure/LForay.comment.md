# LForay.cs

## `public sealed class LForay`

One in-flight recording or transcription search a tenure started for its draft.
It keeps the word, language, target row and scheme the search was asked with, and the cancellation that ends it.
The menu that asked reads those back instead of copying them into fields of its own.
The tenure makes one, cancels the last of its kind, and cancels every open one when it ends.
The search is not awaited by the caller, because the listener hears its end.

## `private readonly LEngine _lEngine;`

The engine the search runs on and the download is asked of.

## `private readonly LTenure _lForayTenure;`

The tenure that started the search, whose draft the pick is written into.

## `private readonly CancellationTokenSource _lForayCancellation = new();`

The one cancellation the search runs under.

## `private bool _lForayCancelled;`

Whether the cancellation was already pulled, so pulling it twice disposes nothing twice.

## `internal LForay(LEngine engine, LTenure tenure, string word, string language, long target, string scheme)`

Made by the tenure alone, with the language read off its draft at that moment.
Whether the pack shows its varieties as flags is asked here once, so a landing row need not ask.

## `public long LForayTarget { get; }`

The pronunciation or transcription row the menu was opened from, zero naming the primary row.

## `public string LForayWord { get; }`

The headword the search was asked for, as the form held it when the menu opened.

## `public string LForayLanguage { get; }`

The draft's language when the search started.

## `public string LForayScheme { get; }`

The transcription scheme searched, or empty for a pronunciation or recording search.

## `public bool LForayFlagged { get; }`

Whether the language shows its varieties as flags, read once at the start.

## `public void LForayCancel()`

Ends the search so nothing further streams in.
A second call is inert.

## `public async Task<bool> LForayRecordingSave(LRecording recording)`

Downloads the taken recording into the workspace and attaches it to the target row.
False when the draft's headword or language no longer matches what was searched for, before or after the download.
A file fetched for a word the form has left stays in the workspace but is attached to nothing.
The primary row takes an audio request and a further row a pronunciation audio request, both applied at once.
Waiting keystrokes are written before each check, so a headword still in the quiet counts as typed.

## `internal void LForayStart(Func<CancellationToken, Task> search, Action finish)`

Runs the search under this foray's cancellation without anyone awaiting it.

## `private async Task LForayRun(Func<CancellationToken, Task> search, Action finish)`

A cancelled search ends quietly, because whoever cancelled it has moved on.
A search that fails otherwise tells its listener it finished, so the menu leaves its searching state.
A foray cancelled before the search began runs nothing.

## `private bool LForayDraftCheck()`

Whether the draft still holds the word and language the search was asked for.
An ended tenure holds no draft and answers false.
