# TRecordingClerk.cs

## `public sealed class TRecordingClerk`

The recording clerk over a real rig with a canned source client.

## `public async Task RecordingClerkFind_PackWithSource_EmitsStepsAndAnswersRecordings()`

A harvest over the pack's one source reaches the listener step by step and answers the same recordings.

## `public void RecordingClerkSweep_UnnamedFile_DeletesItAndKeepsTheStoredOne()`

A file no pronunciation row names is deleted, a file a row names survives.

## `public void RecordingClerkPlay_MissingFile_PlaysNothing()`

A file that is not on disk, or no file at all, never reaches the phonograph.

## `private static string TRecordingFileSave(TWorkspace workspace, string name)`

One byte written under the workspace audio folder, as a stored recording would be.
