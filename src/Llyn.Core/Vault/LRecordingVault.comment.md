# LRecordingVault.cs

## `public interface LRecordingVault`

The port that brings a chosen recording's bytes into the workspace and answers with a local path.
`LRecordingArchive` in Infrastructure is its adapter over the workspace root and the shared `HttpClient`.
The engine holds the port from the rig, so it names neither the folder nor the client.

## `Task<string> LRecordingSave(LRecording recording, string word, string language, CancellationToken cancellation);`

Saves the recording under the language's audio folder and answers with the file it landed on.

## `Task<string> LRecordingPrepare(LRecording recording, CancellationToken cancellation);`

Caches the recording for immediate playback and answers with the cached file.

## `void LRecordingSweep(IReadOnlySet<string> kept);`

Deletes every file under the audio folder whose full path is not in `kept`.
The engine computes `kept` from stored and drafted pronunciations and never walks the folder itself.
