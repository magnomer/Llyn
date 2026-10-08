# LForay.cs
Hash: `8a42cc5bb1840b57`

## `public sealed class LForay`

One in-flight recording or transcription search a tenure started for its draft.
It keeps the word, language, target row and scheme the search was asked with, and the cancellation that ends it.
The menu that asked reads those back instead of copying them into fields of its own.
The tenure's `LErrand` makes one, cancels the last of its kind, and cancels both current ones when the tenure ends.
The search is not awaited by the caller, because the listener hears its end.

## `private readonly LEngine _lEngine;`

The engine the search runs on and the download and the preview fetch are asked of.

## `private readonly LTenure _lForayTenure;`

The tenure that started the search, whose draft the pick is written into.

## `private readonly CancellationTokenSource _lForayCancellation = new();`

The one cancellation the search runs under.

## `private readonly LQuillPronunciation _lForayPronunciation;`

The errand's pronunciation quill, which writes the variety of a saved recording.
It is shared with the errand, so a foray builds no quill of its own.

## `private bool _lForayCancelled;`

Whether the cancellation was already pulled, so pulling it twice disposes nothing twice.

## `internal LForay(LEngine engine, LTenure tenure, LQuillPronunciation pronunciation, string word, string language, long target, string scheme)`

Made by the tenure's `LErrand` alone, with the language read off its draft at that moment.
Whether the pack shows its varieties as flags is asked here once, so a landing row need not ask.

## `public long LForayTarget { get; }`

The pronunciation or transcription row the menu was opened from, zero naming the primary row.

## `public string LForayWord { get; }`

The draft's trimmed headword when the search started.

## `public string LForayLanguage { get; }`

The draft's language when the search started.

## `public string LForayScheme { get; }`

The transcription scheme searched, or empty for a pronunciation or recording search.

## `public bool LForayFlagged { get; }`

Whether the language shows its varieties as flags, read once at the start.

## `public bool LForaySchemed`

Whether a transcription scheme was named, so the search is a transcription lookup.

## `public bool LForayPrimary`

Whether the search was opened from the primary row, whose target is zero.

## `public bool LForayCancelled`

Whether the search was cancelled, so a step it streamed earlier no longer belongs to the current search.
A replaced or stopped search is always cancelled, so a live one is always the current one.

## `public void LForayCancel()`

Ends the search so nothing further streams in.
A second call is inert.

## `public async Task<bool?> LForayRecordingSave(LRecording recording)`

Downloads the taken recording into the workspace and attaches it to the target row.
False when the draft's headword or language no longer matches what was searched for, before or after the download.
A file fetched for a word the form has left stays in the workspace but is attached to nothing.
The primary row takes an audio request and a further row a pronunciation audio request, both applied at once.
The row is then tagged with the recording's variety, as taking a reading tags it.
An untagged recording sends no variety, and the row keeps whatever it had.
Waiting keystrokes are written before each check, so a headword still in the quiet counts as typed.
Null only when the recording clerk answers no path, so nothing was saved.
A refused or timed out download, or a file not written, reaches the caller as the clerk's vault fault.
Nothing here records it, since the gate that shows it records it once.
The fault wraps the cause, so neither the foray nor Conduct names a file or network type.
Any other fault is a bug and reaches the caller too.

## `public Task<string?> LForayRecordingPrepare(LRecording recording)`

Fetches a listed recording into the workspace cache for a preview and answers the local path.
It attaches nothing, and the search's cancellation does not stop it.
Null only when the recording clerk answers no path, so nothing was prepared.
A failed fetch reaches the caller unrecorded, as in `LForayRecordingSave`.
Any other fault is a bug and reaches the caller too.

## `public Task LForayEnsignLoad(Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store)`

Loads the flags of every variety the search's language declares, handing each row to `store`.
A language that shows no flags loads nothing.
A schemed lookup loads nothing either, since a transcription carries no variety to flag.

## `public (bool, string, string) LForayMarkRead()`

The respelling switch and the brackets a found reading shows in, as the settings answer them for the search's language.
A schemed search shows its readings bare, since a scheme is never respelled.

## `public string LForayReadingRead(string? phonetic, string? respelling)`

The text one found reading shows, by the Application rule a stored pronunciation uses.
The switch is the one `LForayMarkRead` answers, so the text and its brackets always agree.

## `internal static string LForayWordRead(LDraft? draft)`

The word a draft is searched for, its headword trimmed, or empty for no draft.
The start and the pick's check share it, so both read the same word.

## `internal void LForayStart(Func<CancellationToken, Task> search, Action unstarted)`

Runs the search under this foray's cancellation without anyone awaiting it.

## `private async Task LForayRun(Func<CancellationToken, Task> search, Action unstarted)`

A cancelled search ends quietly, because whoever cancelled it has moved on.
A search that fails otherwise is written to the engine's fault record, since nobody awaits it to see the failure.
After the record it runs `unstarted`, unless the foray was cancelled meanwhile.
The tenure's `unstarted` ends the sink only when its relay never sent an end.
So a fault before the harvest or lookup started still closes the menu once.
A sink that throws inside `unstarted` is recorded the same way.
It is never retried.
A foray cancelled before the search began runs nothing.

## `private bool LForayDraftCheck()`

Whether the draft still holds the word and language the search was asked for.
An ended tenure holds no draft and answers false.
