namespace Llyn.Core;

public sealed record LEntry(
    long LEntryId,
    string LEntryHeadword,
    string LEntryLanguage,
    int LEntryGrasp,
    string? LEntryFrequency,
    string? LEntryAddedUtc,
    string? LEntryUpdatedUtc);
