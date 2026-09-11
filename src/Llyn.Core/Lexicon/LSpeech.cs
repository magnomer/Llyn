namespace Llyn.Core;

public sealed record LSpeech(
    long LSpeechEntryId,
    int LSpeechPosition,
    long? LSpeechValueId,
    string? LSpeechCustom = null);
