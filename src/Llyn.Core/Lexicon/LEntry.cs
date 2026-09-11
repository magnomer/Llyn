namespace Llyn.Core;

public sealed record LEntry(
    long LEntryId,
    string LEntryHeadword,
    string LEntryLanguage,
    string? LEntryProficiency,
    string? LEntryFrequency,
    string? LEntryAddedUtc,
    string? LEntryUpdatedUtc);
