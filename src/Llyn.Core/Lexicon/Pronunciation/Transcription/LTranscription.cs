namespace Llyn.Core;

public sealed record LTranscription(
    long LTranscriptionId,
    long LTranscriptionEntryId,
    int LTranscriptionPosition,
    string LTranscriptionScheme,
    string LTranscriptionText);
