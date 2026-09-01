namespace Llyn.Core;

public sealed record LSpeech(
    string LSpeechEntryId,
    int LSpeechPosition,
    string? LSpeechValueId,
    string? LSpeechCustom = null);
