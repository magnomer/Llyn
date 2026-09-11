namespace Llyn.Core;

public sealed record LSpeech(
    long LSpeechEntryId,
    int LSpeechPosition,
    string? LSpeechValueId,
    string? LSpeechCustom = null);
