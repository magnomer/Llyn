namespace Llyn.Core;

public sealed record LPronunciationAudio(
    long LPronunciationAudioId,
    string LPronunciationAudioFile,
    string? LPronunciationAudioSource,
    string LPronunciationAudioAdded);
