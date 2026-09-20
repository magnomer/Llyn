# LRecordingArchive.cs

## `public sealed class LRecordingArchive : LRecordingVault`

The adapter behind the recording port: downloads remote recordings into the workspace and returns their local paths.
A chosen recording's bytes are saved under a per-language folder.
A previewed recording's bytes are cached under the workspace's temp folder.
The root and the client are fixed at construction, so the rig builds a fresh one per workspace.
Nothing about which source or language is baked in here.

## `public LRecordingArchive(string root, HttpClient client)`

Binds the archive to the workspace `root` every file lands under and the `client` every fetch goes through.

## `public async Task<string> LRecordingSave(`

Downloads a chosen recording into the workspace and returns the saved path.
The file sits under `audio/<language>/` and is named by the headword, the recording's variety and its address.
A recording carrying no variety leaves the variety part out, so a pack without varieties reads the same.

## `public void LRecordingSweep(IReadOnlySet<string> kept)`

Walks the `audio` folder and deletes every file whose full path is not in `kept`.
A missing folder sweeps nothing.
A file that will not delete is left for the next sweep.

## `private async Task<byte[]> LRecordingArchiveRead(string address, CancellationToken cancellation)`

Fetches the bytes of one recording, the one read both saving and previewing go through.
A host answering 429 is asked again after a pause, up to three tries in all.
The pause is the host's Retry-After when it is longer than five seconds and five seconds otherwise.
Wikimedia's audio host answers 429 to a handful of quick fetches and names one second, which retries proved too short.
Any other failing status raises, so the caller sees a refused fetch rather than an empty file.

## `private static string LRecordingStemRead(string word, string variety, string address)`

The file stem a saved recording takes, before its extension.
A tagged recording appends the variety after a dot, so `tomato.British` and `tomato.American` sit side by side.
A short digest of the address ends the stem, so `Weg` and `weg` never land on one file.
Windows folds case in file names, and two words that differ only by case are still two words.
The same digest keeps two homographs apart when their recordings differ.
One recording fetched twice lands on one file.
Both parts are normalized, so a variety name never carries a path character into the file name.

## `private static string LRecordingDigestRead(string address)`

The hex digest of an address, the identity every cached or saved file is named by.

## `public async Task<string> LRecordingPrepare(LRecording recording, CancellationToken cancellation)`

Downloads a recording to a temporary cache file for immediate playback and returns its path.
Playing a local file is reliable, whereas streaming a remote, token-bearing URL through the media stack is not.
The file is named by a digest of the full address, so two sources' same-named files never collide.
A file already in the cache is returned as it stands, without another fetch.
Rewriting it would fail anyway, since the player keeps the file it last played open.

## `private static string LRecordingExtensionRead(string address)`

The extension the saved file takes, read from the address and kept only when it names an audio format.
Anything else falls back to `.mp3`, so a source cannot name a file `.exe` or `.html` on disk.

## `private static string LRecordingArchiveNormalize(string value)`

A value made safe as one file name segment.
Path characters become underscores.
A name Windows reserves for a device, such as `con` or `nul`, is prefixed.
The file could not be created otherwise.
The reservation covers the part before the first dot, which is why the check reads only that far.

## Inline notes

### `_lRecordingArchiveRoot, LRecordingArchiveBucket, LRecordingArchiveNormalize(language));`

Saved audio is workspace data, so it lives under the chosen workspace root, never elsewhere.

### `string directory = Path.Combine(_lRecordingArchiveRoot, LRecordingArchiveCache);`

Temporary files also stay inside the workspace, under its own temp folder.
