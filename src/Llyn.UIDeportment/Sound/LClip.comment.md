# LClip.cs

## `public sealed class LClip`

The deportment of the recording menu, reading the editor desk's recording search while recordings are searched.
The menu asks it for the search's language, flag and target, and hands it the recording the user picked.
The desk holds the search, so this deportment holds no engine handle.
A new start cancels the last search, and a closed menu cancels it too.
The search streams its steps to a delegate the menu hands in at the start, wrapped for the menu's thread.
The menu hands this deportment's own step handler, so the split into source, recording and end happens here.
Each step becomes one notice, and the menu wires a control write to each notice and decides nothing itself.

## `public event Action<string, int>? LClipSourceStarted;`

An audio source began searching, named with its position in the pack's list.
`LClipRecordingAdded` is a source's one answer, and `LClipFinished` is the end of the whole search.

## `public void LClipStepHandle(CHarvestStep step)`

One step of the search, told apart by the verdicts the step answers.
A step carrying a recording is that source's answer.
A step that is the end closes the search.
Any other step is a source starting.

## `public Task<bool> LClipRecordingSave(CRecording recording)`

Saves the picked recording through the desk's search and answers whether it attached to the draft.
The recording is copied back into the engine's shape first.
With no search running it answers false, since there is nothing to attach to.
