# LEngineRecordingSweep.cs

## `public sealed partial class LEngine`

The sweep that drops recordings nothing names any more.
A headword or language change empties the audio of a draft, and a cancelled draft drops the rest.
The files those fetches saved stay under `audio` until this sweep runs.

## `private const string LEngineRecordingBucket = "audio";`

The workspace folder the recordings are saved under, one subfolder per language.

## `public void LEngineRecordingSweep()`

Deletes every file under `audio` that neither a stored pronunciation nor a draft on disk names.
The window runs it once at open, before any draft is held, so no fetch in flight can race it.
The kept set holds full paths compared without case, which is how the workspace's file system compares them.
A file that will not delete is left for the next open.

## `private void LEngineRecordingPlace(HashSet<string> kept, string file)`

Resolves one stored path against the workspace and puts it in the kept set.
An empty path or one that resolves nowhere places nothing.
