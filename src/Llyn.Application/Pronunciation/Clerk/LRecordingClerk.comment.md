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

## `public bool LRecordingClerkExist(string? file)`

Whether the recording resolves to a file that exists.

## `public int LRecordingClerkPlay(string? file, double volume)`

Plays the recording at `volume` through the phonograph when it resolves to a file that exists.
A missing or unnamed file plays nothing, so the phonograph never opens a path that is not there.
The file is resolved through the trail, so a relative path plays from the workspace root.
Answers a fresh ticket naming this play, or zero when nothing played.
A `volume` that is not a finite number keeps the current level.
The app shares one phonograph, so a caller keeps its ticket to reach only its own play.

## `public void LRecordingClerkStop(int ticket)`

Stops the phonograph only while `ticket` names the latest play.
A view clearing after another view played thus leaves that other sound running.

## `public void LRecordingClerkClear()`

Stops the phonograph whatever play is running.
The engine calls it on the old clerk before a workspace switch.
The phonograph outlives the clerk, so the old play would otherwise keep sounding.
The new clerk's tickets start over, so no ticket from before could reach that play.

## `public void LRecordingClerkAdjust(double volume)`

Sets the phonograph level, clamped so the player never takes a level outside zero to one.
The app has one audio level, so a change reaches whatever plays.
A `volume` that is not a finite number is ignored.

## `public LEntryDraft LRecordingClerkResolve(LEntryDraft draft)`

The draft with every recording path made absolute, so a form can play it.

## `public void LRecordingClerkSweep()`

Deletes every stored recording no pronunciation row and no draft names.
Paths are compared as workspace-relative files, so a moved workspace is swept correctly.

## `private void LRecordingPlace(HashSet<string> kept, string file)`

Adds the workspace-relative form of `file` to `kept` when it resolves to a file.

## `private IReadOnlyList<LSource> LHarvestSourceRead(string language, LLanguage pack)`

The harvest sources of `language`, built from `pack` on first use.
