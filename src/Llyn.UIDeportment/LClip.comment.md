# LClip.cs

## `public sealed class LClip`

The deportment of the recording menu: the foray it holds over the editor's tenure while recordings are searched.
The menu asks it for the foray's language, flag and target, and hands it the recording the user picked.
The foray belongs to the tenure.
A new hold cancels the last one, and a closed menu cancels it too.
The editor deportment starts the foray over its desk and hands it here, so no desk is held.
The listener the foray streams to is the menu itself, handed in at the start.

## `public Task<bool> LClipRecordingSave(LRecording recording)`

Saves the picked recording through the foray and answers whether it attached to the draft.
With no foray held it answers false, since there is nothing to attach to.
