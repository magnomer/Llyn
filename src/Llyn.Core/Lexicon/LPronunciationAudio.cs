namespace Llyn.Core;

/// <summary>
/// The recorded audio a pronunciation owns: one downloaded file per pronunciation, stored as a path
/// relative to the workspace folder so moving or copying a workspace keeps its audio. The owning
/// pronunciation id is the identity — at most one audio row per pronunciation — so saving replaces
/// whatever file the pronunciation had.
/// </summary>
/// <param name="LPronunciationAudioId">Owning pronunciation id — the row's identity.</param>
/// <param name="LPronunciationAudioFile">Audio file path, relative to the workspace folder.</param>
/// <param name="LPronunciationAudioSource">Optional label of the source the recording came from.</param>
/// <param name="LPronunciationAudioAdded">Time the file was recorded into the workspace, ISO 8601 UTC.</param>
public sealed record LPronunciationAudio(
    string LPronunciationAudioId,
    string LPronunciationAudioFile,
    string? LPronunciationAudioSource,
    string LPronunciationAudioAdded);
