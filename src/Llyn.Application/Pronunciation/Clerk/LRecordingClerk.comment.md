# LRecordingClerk.cs

## `public sealed class LRecordingClerk`

Recordings harvested, stored, resolved and swept for one rig.
The harvest sources of each language are built once from the pack and kept until the rig changes.
The session trove that remembers a harvest stays in the engine, so the clerk only scans and publishes.

## `public LRecordingClerk(LRig rig, LLanguageCache languages, LTrailClerk trail, LClaimClerk claims)`

Reads the recording and pronunciation ports and the source factory out of `rig`.
The claim clerk supplies the held drafts the sweep must keep.

## `public Task<IReadOnlyList<LRecording>> LRecordingClerkFind(string word, string language, string variety, LListener listener, CancellationToken cancellation)`

A harvest of `word` over the language's harvest sources, its steps sent to `listener`.

## `public static Task LRecordingClerkPublish(IReadOnlyList<LRecording> held, string variety, LListener listener)`

Replays a remembered harvest to `listener`, filtered to `variety`, and finishes it.

## `public Task<string> LRecordingClerkSave(LRecording recording, string word, string language, CancellationToken cancellation)`

Stores a harvested recording under the workspace and answers its path.

## `public Task<string> LRecordingClerkPrepare(LRecording recording, CancellationToken cancellation)`

Fetches a recording to a playable local file without storing it.

## `public string LRecordingClerkResolve(string file)`

The stored recording path as an absolute local path.

## `public bool LRecordingClerkExist(string? file)`

Whether the recording resolves to a file that exists.

## `public LEntryDraft LRecordingClerkResolve(LEntryDraft draft)`

The draft with every recording path made absolute, so a form can play it.

## `public void LRecordingClerkSweep()`

Deletes every stored recording no pronunciation row and no draft names.
Paths are compared as workspace-relative files, so a moved workspace is swept correctly.

## `private void LRecordingPlace(HashSet<string> kept, string file)`

Adds the workspace-relative form of `file` to `kept` when it resolves to a file.

## `private IReadOnlyList<LSource> LHarvestSourceRead(string language, LLanguage pack)`

The harvest sources of `language`, built from `pack` on first use.
