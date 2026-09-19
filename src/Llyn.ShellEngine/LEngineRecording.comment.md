# LEngineRecording.cs

## `public sealed partial class LEngine`

The pronunciation lookup and the recording discovery, the two searches a draft's menu opens.
Each takes what it needs under the gate, then runs the fetch outside it.
What a search found is kept in the trove under the asking draft, so reopening the menu replays it.
The transcription and recording sources of each language are built once here and cached apart.

## `public Task LEnginePronunciationFind(long session, string word, string language, LReceiver receiver, CancellationToken cancellation)`

Starts a pronunciation lookup for `word` in `language` and streams results to `receiver`.
The task completes when every source finishes.
`session` is the draft the asking editor holds, and it names the trove the answer is kept in.
A lookup already answered under that draft is replayed instead of searched again.
So reopening the menu on an unchanged headword costs no network at all.
When the pack declares respelling groups, the receiver is wrapped in `LReceiverRespelling`, whatever the switch says.
Both the fresh search and the replay stream through that wrapper, so every candidate carries both forms.
The trove is never wrapped and keeps cleaned but un-respelled text.
The switch is the phonetician's alone, picking which of the two forms each row shows.
The pack is read under the gate whatever the switch says, because the fresh path needs its cleanup groups.

## `internal Task LEngineRecordingFind(long session, string word, string language, long target, LListener listener, CancellationToken cancellation)`

Starts an audio-recording discovery for `word` in `language` and streams results to `listener`.
The task completes when every source finishes.
`target` is the pronunciation row the menu was opened from, and `0` names the primary row.
The row's variety is read from the draft here, so the shell never decides which case applies.
An empty variety returns every variety, tagged rows under their tag and untagged rows fanned out per declared variety.
A named variety returns that variety only, an untagged row collapsing to one under it.
It reuses what the same draft already found, narrowed to the row's variety, so no row's menu fetches twice.
The pack is read under the gate, because the fresh path needs its declared varieties.

## `private string LEngineVarietyResolve(long session, long target)`

The variety of the pronunciation row `target` in draft `session`, trimmed, or empty when there is none.
`target` of `0` reads the primary row, the first in the draft's list.
A draftless session, a missing draft, or a row no longer present all read as no variety.
So the menu still opens and shows every variety rather than failing.

## `private async Task LEngineCandidateScan(long session, string word, string language, IReadOnlyList<LSource> sources, LLanguage pack, LReceiver receiver, CancellationToken cancellation)`

Runs a real lookup and keeps what it returned in the trove under `session`.
The lookup is handed the pack's declared varieties, so untagged readings fan out per variety.
It is handed the pack's cleanup groups too, so what reaches the trove is already cleaned.
The receiver still sees each candidate stream in, because the search is unchanged.
Nothing is kept when the search is cancelled, since the task then ends by throwing.

## `private async Task LEngineRecordingScan(long session, string word, string language, string variety, IReadOnlyList<LSource> sources, LLanguage pack, LListener listener, CancellationToken cancellation)`

The recording counterpart of `LEngineCandidateScan`.
The harvest gets the pack's declared varieties and the row's variety, so it fans out and narrows its stream.
What it returns holds every variety, and that whole set is kept in the trove for any row to narrow.

## `private static Task LEngineCandidatePublish(IReadOnlyList<LCandidate> held, LReceiver receiver)`

Hands a kept answer to the receiver in one pass, then reports the search finished.
The receiver cannot tell a replay from a search, so the menu needs no second path.
The task is already complete, because nothing was awaited.

## `private static Task LEngineRecordingPublish(IReadOnlyList<LRecording> held, LListener listener)`

The recording counterpart of `LEngineCandidatePublish`.
The caller narrows the kept set to the opening row's variety first, as the harvest narrows a live stream.

## `internal Task<string> LEngineRecordingSave(LRecording recording, string word, string language, CancellationToken cancellation)`

Downloads the chosen `recording` into the workspace through the recording port and returns the saved path.
The recording carries its own variety, and the archive names the file by it.
The port is read under the gate and called outside it, so a rig swap mid-download changes nothing.

## `public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)`

Downloads the `recording` to a temporary file for playback through the recording port and returns its path.

## Inline notes

### `private string LEngineRecordingFormat(string path)`

The workspace-relative form of a downloaded recording's path, which is how it is stored.
A path outside the workspace has no relative form and is stored as it stands.

### `private IReadOnlyList<LSource> LEngineLookupRead(string language)`

The transcription sources of `language`, built from its pack on first use and cached by name.
The cache is cleared with the workspace, since the pack it was built from belongs to that folder.

### `private IReadOnlyList<LSource> LEngineHarvestRead(string language)`

The recording sources of `language`, built and cached the same way, apart from the transcription ones.
A lookup never reaches an audio source and a download never reaches a transcription one.
