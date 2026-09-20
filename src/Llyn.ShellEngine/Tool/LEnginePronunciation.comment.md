# LEnginePronunciation.cs

## `public sealed partial class LEngine`

The pronunciation facades: rows, notes, recordings, lookups, transcriptions and frequencies.
Each takes the gate and calls the pronunciation, recording, transcription or frequency clerk.
The session trove that remembers a lookup or harvest stays here, since a session is an engine fact.

## `internal LPronunciation LEnginePronunciationCreate(LPronunciation pronunciation)`

One pronunciation row created through the clerk.

## `internal IReadOnlyList<LPronunciation> LEnginePronunciationRead(long entryId)`

The stored pronunciation rows of an entry.

## `public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order)`

The pronunciation catalog rows matching `query`, sorted.

## `public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The same rows with the language filter applied.

## `public IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(LVista vista)`

The rows of a vista, twinned names and the chosen mark applied through the vista build.

## `internal void LEnginePronunciationUpdate(LPronunciation pronunciation)`

One row rewritten through the clerk.

## `internal void LEnginePronunciationDelete(long id)`

One row deleted through the clerk.

## `internal void LEngineAudioSave(long pronunciationId, string file, string? source)`

The recording of one row saved, stored workspace-relative.

## `internal LPronunciationAudio? LEngineAudioRead(long pronunciationId)`

The recording of one row with its path made absolute through the recording clerk.

## `internal void LEngineNoteSave(LNote note)`

The note of an entry saved, normalised.

## `internal LNote? LEngineNoteRead(long entryId)`

The note of an entry.

## `internal void LEngineNoteDelete(long entryId)`

The note of an entry deleted.

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

## `public IReadOnlyList<string> LEngineSchemeRead(string language)`

The scheme names of a language.

## `internal Task LEngineTranscriptionFind(long session, string word, string language, string scheme, Action<LLookupStep> sink, CancellationToken cancellation)`

The lookup of `word` under one scheme, replayed from the trove when the session already asked.

## `internal IReadOnlyList<LTranscription> LEngineTranscriptionRead(long entryId)`

The stored transcription rows of an entry.

## `internal IReadOnlyList<LTranscription> LEngineTranscriptionSet(long entryId, IReadOnlyList<LTranscription> transcriptions)`

The rows of an entry replaced.

## `internal Task<IReadOnlyList<LFrequency>> LEngineFrequencyFind(string word, string language, CancellationToken cancellation)`

The frequencies of `word` fetched now.

## `internal string? LEngineBandResolve(string language, string source, string raw)`

The band of a raw figure under the source's declaration.

## `public IReadOnlyList<LFrequency> LEngineFrequencyRead(long entryId)`

The stored frequency rows of an entry, regraded, a fetch started when there are none.

## `internal void LEngineFrequencyStart(long entryId)`

Starts the frequency fetch of an entry.
