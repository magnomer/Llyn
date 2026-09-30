# LPronunciationFacade.cs

## `internal sealed class LPronunciationFacade`

The engine's facade for pronunciation rows, notes, recordings, lookups, transcriptions and frequencies.
Each takes the gate and calls the pronunciation, recording, transcription or frequency clerk.
The session trove that remembers a lookup or harvest stays here, since a session is an engine fact.

## `public LPronunciationFacade(LEngine engine)`

Stores the engine and its gate.

## `public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order)`

The pronunciation catalog rows matching `query`, sorted.

## `public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The same rows with the language filter applied.

## `public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(LVista vista)`

The rows of a vista, twinned names and the chosen mark applied through the vista build.

## `public Task LEnginePronunciationFind(long session, string word, string language, Action<LLookupStep> sink, CancellationToken cancellation)`

The pronunciation lookup of `word`, its steps sent to `sink`.
A session that already looked the word up is replayed from the trove instead.
The lookup is started under the gate and awaited outside it.

## `private async Task LEngineTroveSave(Task<IReadOnlyList<LCandidate>> scan, long session, string word, string language, string? scheme)`

Remembers the candidates a finished lookup found under the session, plain or per scheme.

## `internal Task LEngineRecordingFind(long session, string word, string language, long target, Action<LHarvestStep> sink, CancellationToken cancellation)`

The recording harvest of `word`, filtered to the variety of the draft row `target` names.
A session that already harvested the word is replayed from the trove instead.

## `private async Task LEngineTroveSave(Task<IReadOnlyList<LRecording>> scan, long session, string word, string language)`

Remembers the recordings a finished harvest found under the session.

## `private string LEngineVarietyResolve(long session, long target)`

The variety of the draft row `target` names, the first row's for zero, empty outside a session.

## `internal Task<string> LEngineRecordingSave(LRecording recording, string word, string language, CancellationToken cancellation)`

Stores a harvested recording under the workspace and answers its path.

## `public Task<string> LEngineRecordingPrepare(LRecording recording, CancellationToken cancellation)`

Fetches a recording to a playable local file without storing it.

## `public void LEngineRecordingSweep()`

Deletes every stored recording no row and no draft names.

## `public bool LEngineRecordingExist(string? file)`

Whether the recording resolves to a file that exists.

## `public int LEngineRecordingPlay(string? file, double volume)`

Plays the recording at `volume` under the gate, and plays nothing for a file that does not exist.
Answers the play's ticket, or zero when nothing played.

## `public int LEngineRecordingPlay(LEntryDraft draft, double volume)`

Plays the draft's own recording, the audio of its first pronunciation, as the file form does.

## `public (bool, bool) LEnginePlaybackRead(LEntryDraft draft)`

Whether the draft's own recording exists, and whether the view has anything to play at all.
The second holds for the own recording or for any accent row that is notated and carries audio.

## `public (string?, bool) LEngineAudioRead(LEntryDraft draft)`

The draft's own recording while its file exists, else null, and whether the editor has anything to play.
The second holds for the own recording or for any accent row that carries audio.
The editor shows every accent row, so unlike the reading view it counts rows without a notation too.

## `public Uri? LEngineAudioResolve(string? file)`

The address the editor's player opens for a stored recording, or null when the file does not exist.

## `public void LEngineRecordingStop(int ticket)`

Stops the playing recording while `ticket` names it.
It skips the gate, since it touches only the phonograph and never waits on a long engine call.

## `public void LEngineVolumeSet(double volume)`

Sets the playing level without the gate, for the same reason as a stop.

## `public IReadOnlyList<string> LEngineSchemeRead(string language)`

The scheme names of a language.

## `internal Task LEngineTranscriptionFind(long session, string word, string language, string scheme, Action<LLookupStep> sink, CancellationToken cancellation)`

The lookup of `word` under one scheme, replayed from the trove when the session already asked.

## `public LFrequencyGauge? LEngineFrequencyResolve(long entryId, string once)`

The entry's frequency gathered into one answer, or null when it has none.
The `once` text words a word interval.

## `internal void LEngineFrequencyStart(long entryId)`

Starts the frequency fetch of an entry.
