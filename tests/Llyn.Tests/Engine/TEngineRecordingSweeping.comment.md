# TEngineRecordingSweeping.cs

## `public sealed class TEngineRecordingSweeping`

Facts for the sweep that drops recording files nothing names.

## `public void RecordingSweep_UnnamedFile_Deletes()`

A file under `audio` that no pronunciation and no draft names is deleted.

## `public void RecordingSweep_StoredFile_Stays()`

A file a stored pronunciation names survives the sweep.
The path is stored relative to the workspace, so the sweep resolves it before comparing.

## `public void RecordingSweep_DraftFile_Stays()`

A file a draft on disk names survives the sweep, since a held draft may still commit it.

## `private static string TRecordingFileSave(TWorkspace workspace, string name)`

Writes one byte under the workspace's `audio/english` and answers the full path.
