# LClip.cs

## `public sealed class LClip`

The deportment of the recording menu: the foray it holds over the editor's tenure while recordings are searched.
The menu asks it for the foray's language, flag and target, and hands it the recording the user picked.
The foray belongs to the tenure.
A new hold cancels the last one, and a closed menu cancels it too.
The editor deportment starts the foray over its desk and hands it here, so no desk is held.
The foray streams its steps to a delegate the menu hands in at the start, wrapped for the menu's thread.
The menu hands this deportment's own step handler, so the split into source, recording and end happens here.
Each step becomes one notice, and the menu wires a control write to each notice and decides nothing itself.

## `public event Action<string, int>? LClipSourceStarted;`

An audio source began searching, named with its position in the pack's list.
`LClipRecordingAdded` is a source's one answer, and `LClipFinished` is the end of the whole search.

## `public void LClipStepHandle(LHarvestStep step)`

One step of the search, told apart by the verdicts the step answers.
A step carrying a recording is that source's answer.
A step that is the end closes the search.
Any other step is a source starting.

## `public Task<bool> LClipRecordingSave(LRecording recording)`

Saves the picked recording through the foray and answers whether it attached to the draft.
With no foray held it answers false, since there is nothing to attach to.
